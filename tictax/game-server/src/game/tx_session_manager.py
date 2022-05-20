import jwt
import os
import json
import settings
from src.metabase import *
from src.game.tx_session import *
from src.game.player import Player

class TxSessionManager(metaclass=MetaBase):
    """ TxSessionManager will be creating game session when
    a player creates a new match and from there handle all 
    game related messages. It will also handle join, close
    and destroy session messages.  
    """

    def __init__(self) -> None:
        self.__logger: Log

        self.tx_sessions: dict[str, TxSession] = {}

        # client_id --> player object
        self.connected_players: dict[int, Player] = {} 

        # Symmetric key used to verify signatures for JWT token
        self.sym_key = settings.TICTAX_JWT_KEY

    def is_token_expired(self, client_username: str) -> bool:
        pass

    def force_disconnect_client(self, client: dict, server):
        self.__logger.warning(f"[TxSessionmanager] -> Force disconnected client {client['id']}")
        self.connected_players.pop(client['id'], None)
        server.disconnect_client(client)

    def handle_message(self, client: dict, server, message: str) -> None:
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

            player: Player = self.connected_players.get(client['id'], None) 

            if msg_type == 'auth':

                try:
                    print(self.sym_key)
                    json_decoded = jwt.decode(json_data['token'], self.sym_key, algorithms=["HS512"])
                    player = Player(
                        client['id'],
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
                self.force_disconnect_client(client, server)

            # ==========================================================

        except Exception as ex:

            # Check if the player was authenticated
            if not client['id'] in self.connected_players:
                server.disconnect_client(client)

            self.__logger.exception(f"[TxSessionManager] -> Error in handle_message() | ClientId {client['id']} received message: " + message)






