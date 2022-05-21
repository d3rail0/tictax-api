from typing import Set
import jwt
import json
import settings
from src.metabase import *
from src.game.tx_session import *
from src.game.player import Player
import src.game.protocol.msg_types as mtypes
from src.game.db_manager import DBManager

import threading

class TxSessionManager(metaclass=MetaBase):
    """ TxSessionManager will be creating game session when
    a player creates a new match and from there handle all 
    game related messages. It will also handle join, close
    and destroy session messages.  
    """

    sessions_lock = threading.Lock()

    def __init__(self) -> None:
        self.__logger: Log

        self.db_manager = DBManager()

        self.tx_sessions: dict[int, TxSession] = {}

        # client_id --> player object
        self.connected_players: dict[int, Player] = {} 
        self.connected_usernames: set = set()

        # Symmetric key used to verify signatures for JWT token
        self.sym_key = settings.TICTAX_JWT_KEY

    def force_disconnect_client(self, client: dict, server):
        self.__logger.warning(f"[TxSessionmanager] -> Force disconnected client {client['id']}")
        server.disconnect_client(client)

    def on_client_disconnected(self, client: dict, server) -> None:
        self.leave_match(client['id'], server)
        if client['id'] in self.connected_players:
            self.connected_usernames.discard(self.connected_players.get(client['id']).username)
            self.connected_players.pop(client['id'])

    def create_match(self, player: Player, server, json_data: dict) -> None:

        if player.is_playing():
            # Player is already playing so 
            # we don't allow them to create new matches
            player.send_error("You are already active in another match!");
            return

        print(f"{player.username} is playing => {player.is_playing()}")

        with self.sessions_lock:
            # Create a TxSession object and assign current player
            session_id = self.db_manager.create_match_ret_id(player)
            player.set_active(session_id)

            new_tx_session = TxSession(player)
            self.tx_sessions[new_tx_session.id] = new_tx_session

        self.__logger.info(f"New match with id {new_tx_session.id} has been created by {player.username}")
    
    def join_match(self, player: Player, server, json_data: dict) -> None:
        
        if player.is_playing():
            # Player is already playing so 
            # we don't allow them to join another match
            player.send_error("You are already active in another match!");
            return

        j_match: mtypes.JoinMatch = mtypes.apply_schema_conv(mtypes.JoinMatch, json_data)
        
        with self.sessions_lock:
            # Check if specified match_id exists
            if not j_match.matchId in self.tx_sessions:
                player.send_error(f"There is no active match with ID: {j_match.matchId}");
                return

            tx_session = self.tx_sessions[j_match.matchId]

            # Check if the specified match already has 2 players playing
            if tx_session.is_active():
                player.send_error(f"Two players have already joined the match");
                return

            try:            
                tx_session.player_joined(player, server)
                self.db_manager.opponent_join(tx_session.id, player)

                player.set_active(tx_session.id)
            except Exception as ex:
                player.send_error(str(ex))
                self.__logger.exception(f"Cannot join the match. Message: {json_data}")

    def leave_match(self, client_id, server) -> None:
        # Check this client's opponent and notify them about 
        # the leave.
        try:
            player = self.connected_players.get(client_id, None)

            if player is not None and player.is_playing():

                self.__logger.info(f"{player.username} left match {player.active_tx_session_id}")
                
                with self.sessions_lock:
                    tx_session = self.tx_sessions.get(player.active_tx_session_id)

                player.set_inactive()

                if tx_session.owner.client_id == player.client_id:
                    # Owner has left the match

                    tx_id = tx_session.id
                    
                    with self.sessions_lock:
                        self.tx_sessions.pop(tx_id, None)
                        self.db_manager.remove_match(tx_id)

                    if tx_session.is_active():
                        self.__logger.info(f"{player.username} has closed their match")
                        disconn_msg = mtypes.PlayerDisonnected(tx_id, True)
                        tx_session.player2.ws_handler.send_message(disconn_msg.to_json())
                else:
                    # Opponent of the owner left the match
                    tx_session.handle_opponent_disconnect(server)
                
        except Exception as ex:
            self.__logger.exception(f"Error occured when player: {client_id} left")

    def handle_message(self, raw_client: dict, server, message: str) -> None:
        # Create match --> creates new session
        # Join match --> joins active session by id
        # Authenticate --> should only contain jwt token
        # Every other message shall contain session_id and should be related to game logic

        player: Player = self.connected_players.get(raw_client['id'], None) 

        try:
            # Authentication
            # ==========================================================
            
            # Convert message to a JSON object
            json_data = json.loads(message)
            msg_type = json_data['type']

            if msg_type == mtypes.AUTH:

                try:
                    auth_details: mtypes.Auth = mtypes.apply_schema_conv(mtypes.Auth, json_data)
                    json_decoded = jwt.decode(auth_details.token, self.sym_key, algorithms=["HS512"])
                    player = Player(
                        raw_client,
                        json_decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'], 
                        json_decoded['exp']
                    )

                    if player.username in self.connected_usernames:
                        # The same player already has an open websocket
                        # connection. Setting player = None will cause
                        # forced disconnect only for this session.
                        player = None
                    else:
                        self.connected_usernames.add(player.username)
                        self.connected_players[player.client_id] = player
                        self.__logger.info(f"{player.username} was successfully authenticated!")
                        return

                except Exception:
                    self.__logger.exception(f"[TxSessionManager] -> Authentication failed!")
            
            if player is None:
                # Client sent their first message and it didn't
                # contain authentication token
                self.force_disconnect_client(raw_client, server)
                return

            if player.is_token_expired():
                player.send_error("Token expired")
                self.force_disconnect_client(raw_client, server)
                return

            # ==========================================================

            if msg_type == mtypes.CREATE_MATCH:
                self.create_match(player, server, json_data)
            elif msg_type == mtypes.JOIN_MATCH:
                self.join_match(player, server, json_data)
            elif msg_type == mtypes.LEAVE_MATCH:
                self.leave_match(player.client_id, server)
            else:
                with self.sessions_lock:
                    tx_session = self.tx_sessions.get(player.active_tx_session_id, None)
                
                if tx_session is not None:
                    tx_session.handle_game_message(player, server, msg_type, json_data)
                else:
                    self.__logger.warning(f"Unrecognized message from {player.username}: {json_data}")

        except Exception as ex:

            # Check if the player was authenticated
            if not raw_client['id'] in self.connected_players:
                server.disconnect_client(raw_client)
            elif player is not None:
                # Send error message back to client
                try:
                    player.send_error(str(ex))
                except:
                    pass

            self.__logger.exception(f"[TxSessionManager] -> Error in handle_message() | ClientId {raw_client['id']} received message: " + message)






