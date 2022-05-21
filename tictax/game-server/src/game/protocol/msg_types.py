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
PLAY_MOVE = 'move'
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
    matchId: int

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
    matchId: int
    board: list[list[str]]
    validMoves: list[int]
    opponent: str

    def __init__(self, tx_game, opponent="") -> None:
        super().__init__()

        self.type = GAME_STATE
        self.matchId = tx_game.match_id
        self.board = tx_game.board
        self.validMoves = list(tx_game.available_moves())
        self.opponent = opponent

@dataclass
class PlayerDisonnected(JsonSchemaMixin):
    type: str
    matchId: int
    # goto lobby is false when this message
    # is sent to an owner of a session because
    # their opponent had left the match
    gotoLobby: bool

    def __init__(self, match_id: int, goto_lobby:bool) -> None:
        super().__init__()
        self.type = PLAYER_DISCONNECTED
        self.matchId = match_id
        self.gotoLobby = goto_lobby

@dataclass
class PlayMove(JsonSchemaMixin):
    type: str
    matchId: int
    cell: int

@dataclass
class GameEnd(JsonSchemaMixin):
    type: str
    matchId: int
    winner: str
    isTie: bool

    # username -> win count
    score: dict[str, int]

    def __init__(self, match_id, winner, is_tie, score: dict[str, int]) -> None:
        super().__init__()
        self.type = GAME_END
        self.matchId = match_id
        self.winner = winner
        self.isTie = is_tie
        self.score = score