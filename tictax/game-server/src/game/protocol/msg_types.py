from dataclasses import dataclass
from dataclasses_jsonschema import JsonSchemaMixin, FieldEncoder

# Constants
# ----------------------------------
AUTH         = 'auth'
CREATE_MATCH = 'create'
JOIN_MATCH   = 'join'
LEAVE_MATCH  = 'leave'
ERROR        = 'error'
# ----------------------------------

def apply_schema_conv_multi(type:JsonSchemaMixin, results:dict) -> dict[str, type]:
    return dict([(k, type.from_dict(v)) for k,v in results.items()])
    
def apply_schema_conv(type:JsonSchemaMixin, results:dict) -> type:
    return type.from_dict(results)

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