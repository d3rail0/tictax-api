from dataclasses import dataclass
from dataclasses_jsonschema import JsonSchemaMixin, FieldEncoder

# Constants
# ----------------------------------
AUTH         = 'auth'
CREATE_MATCH = 'create'
JOIN_MATCH   = 'join'
LEAVE_MATCH  = 'leave'
ERROR        = 'error'
GAME_STATE   = 'gs'
GAME_END     = 'ge'
PLAYER_DISCONNECTED = 'p_disconn'
# ----------------------------------

def apply_schema_conv_multi(type:JsonSchemaMixin, results:dict) -> dict[str, type]:
    return dict([(k, type.from_dict(v)) for k,v in results.items()])
    
def apply_schema_conv(type:JsonSchemaMixin, data:dict) -> type:
    return type.from_dict(data)

@dataclass
class Auth(JsonSchemaMixin):
    type: str
    token: str

@dataclass
class JoinMatch(JsonSchemaMixin):
    type: str
    match_id: int

@dataclass
class ErrorMessage(JsonSchemaMixin):
    type: str
    message: str

    def __init__(self, error_message: str) -> None:
        super().__init__()
        self.type = ERROR
        self.message = error_message

@dataclass
class GameState(JsonSchemaMixin):
    type: str
    match_id: int
    board: list[list[str]]
    valid_moves: list[int]

    def __init__(self, tx_game) -> None:
        super().__init__()

        self.type = GAME_STATE
        self.match_id = tx_game.match_id
        self.board = tx_game.board
        self.valid_moves = list(tx_game.available_moves())

@dataclass
class PlayerDisonnected(JsonSchemaMixin):
    type: str
    match_id: int
    # goto lobby is false when this message
    # is sent to an owner of a session because
    # their opponent had left the match
    goto_lobby: bool

    def __init__(self, match_id: int, goto_lobby:bool) -> None:
        super().__init__()
        self.type = OPONNENT_DISCONNECTED
        self.match_id = match_id
        self.goto_lobby = goto_lobby