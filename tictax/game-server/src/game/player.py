
class Player:

    def __init__(self, client_id: int, username: str, token_expiration: int) -> None:
        self.client_id = client_id
        self.username = username
        self.token_expiration = token_expiration