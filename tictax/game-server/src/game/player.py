from websocket_server import WebSocketHandler
import time

class Player:

    @property
    def client_id(self) -> int:
        return self.client_raw['id']

    @property
    def ws_handler(self) -> WebSocketHandler:
        return self.client_raw['handler']

    def __init__(
                self, client_raw: dict, username: str,
                token_expiration: int, 
                active_tx_session_id: int = 0) -> None:

        self.client_raw = client_raw
        self.username = username
        self.token_expiration = token_expiration
        
        self.set_active(active_tx_session_id)

    def is_playing(self) -> bool:
        return self.active_tx_session_id > 0

    def set_inactive(self) -> None:
        """ Sets player to inactive state, meaning
        is_playing() will be returning false onwards until
        player joins another session. 
        """
        self.active_tx_session_id = 0

    def set_active(self, tx_session_id) -> None:
        self.active_tx_session_id = tx_session_id

    def is_token_expired(self) -> bool:
        return int(time.time()) > self.token_expiration