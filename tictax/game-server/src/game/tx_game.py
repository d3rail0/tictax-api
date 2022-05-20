from src.metabase import MetaBase
from global_logger import Log

class TxGame(metaclass=MetaBase):
    """ Implements TicTacToe game logic
    """

    def __init__(self) -> None:
        self.__logger: Log