from websocket_server import WebSocketHandler

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
        self.active_tx_session_id = active_tx_session_id

    def is_playing(self):
        return self.active_tx_session_id > 0