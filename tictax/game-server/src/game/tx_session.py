from global_logger import Log

from src.metabase import MetaBase
from src.game.tx_game import TxGame

class TxSession(metaclass=MetaBase):
    """ TxSession is a game session which will
    create a brand new TxGame object and map all
    game logic to adequate websocket messages.    
    """

    def __init__(self) -> None:
        self.__logger: Log
        
    def on_client_disconnect(self, client_id: int) -> None:
        pass
    