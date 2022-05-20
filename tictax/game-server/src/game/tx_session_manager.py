import jwt
import os
import json
import settings
from src.metabase import *
from src.game.tx_session import *
from src.game.player import Player
import src.game.protocol.msg_types as mtypes

class TxSessionManager(metaclass=MetaBase):
    """ TxSessionManager will be creating game session when
    a player creates a new match and from there handle all 
    game related messages. It will also handle join, close
    and destroy session messages.  
    """

    def __init__(self) -> None:
        self.__logger: Log

        self.session_id = 1;

        self.tx_sessions: dict[int, TxSession] = {}

        # client_id --> player object
        self.connected_players: dict[int, Player] = {} 

        # Symmetric key used to verify signatures for JWT token
        self.sym_key = settings.TICTAX_JWT_KEY

    def is_token_expired(self, client_username: str) -> bool:
        pass

    def force_disconnect_client(self, client: dict, server):
        self.__logger.warning(f"[TxSessionmanager] -> Force disconnected client {client['id']}")
        server.disconnect_client(client)

    def on_client_disconnected(self, client: dict, server) -> None:
        # Check this client's opponent and notify them about 
        # the leave
        self.connected_players.pop(client['id'], None)

    def send_error(self, player: Player, message: str) -> None:
        self.__logger.warning(f"Sending error message to '{player.username}' -> {message}")
        err_msg = mtypes.ErrorMessage(message)
        player.ws_handler.send_message(err_msg.to_json())

    def create_match(self, player: Player, server, json_data: dict) -> None:

        if player.is_playing():
            # Player is already playing so 
            # we don't allow him to create new matches
            self.send_error(player, "You are already active in another match!");
            return

        # Create a TxSession object and assign current player
        player.active_tx_session_id = self.session_id


        self.session_id += 1
    
    def join_match(self, player: Player, server, json_data: dict) -> None:
        pass
    
    def leave_match(self, player: Player, server, json_data: dict) -> None:
        pass


    def handle_message(self, raw_client: dict, server, message: str) -> None:
        # Create match --> creates new session
        # Join match --> joins active session by id
        # Authenticate --> should only contain jwt token
        # Every other message shall contain session_id and should be related to game logic

        try:
            # Authentication
            # ==========================================================
            
            # Convert message to a JSON object
            json_data = json.loads(message)
            msg_type = json_data['type']

            player: Player = self.connected_players.get(raw_client['id'], None) 

            if msg_type == mtypes.AUTH:

                try:
                    auth_details: mtypes.Auth = mtypes.apply_schema_conv(mtypes.Auth, json_data)
                    json_decoded = jwt.decode(auth_details.token, self.sym_key, algorithms=["HS512"])
                    player = Player(
                        raw_client,
                        json_decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'], 
                        json_decoded['exp']
                    )

                    self.connected_players[player.client_id] = player
                    self.__logger.info(f"{player.username} was successfully authenticated!")
                    return

                except Exception:
                    self.__logger.exception(f"[TxSessionManager] -> Authentication failed!")
            
            if player is None:
                # Client sent their first message and it didn't
                # contain authentication token
                self.force_disconnect_client(raw_client, server)

            # ==========================================================

            if msg_type == mtypes.CREATE_MATCH:
                self.create_match(player, server, json_data)
            elif msg_type == mtypes.JOIN_MATCH:
                self.join_match(player, server, json_data)
            elif msg_type == mtypes.LEAVE_MATCH:
                self.leave_match(player, server, json_data)
            else:
                # Pass the message to an actual session
                pass


        except Exception as ex:

            # Check if the player was authenticated
            if not raw_client['id'] in self.connected_players:
                server.disconnect_client(raw_client)

            self.__logger.exception(f"[TxSessionManager] -> Error in handle_message() | ClientId {raw_client['id']} received message: " + message)






