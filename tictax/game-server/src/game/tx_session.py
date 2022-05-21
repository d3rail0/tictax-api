from global_logger import Log

from src.metabase import MetaBase
from src.game.tx_game import TxGame
from src.game.player import Player
import src.game.protocol.msg_types as mtypes

from random import randrange


class TxSession(metaclass=MetaBase):
    """ TxSession is a game session which will
    create a brand new TxGame object and map all
    game logic to adequate websocket messages.    
    """

    symbols = ['X', 'O']

    @property
    def owner(self) -> Player:
        return self.player1

    def __init__(self, owner: Player) -> None:
        self.__logger: Log        

        self.id = owner.active_tx_session_id
        self.game = TxGame(self.id)

        self.player1: Player = owner
        self.player2: Player = None 

        self.__randomize_symbols()

    def __randomize_symbols(self) -> None:
        rand_num = randrange(0, 2)
        self.player1_symbol = self.symbols[rand_num]
        self.player2_symbol = self.symbols[(rand_num+1) % 2]
    
    def is_active(self) -> bool:
        """ Returns true if two players had already joined
        the same session.
        """
        return self.player2 is not None and self.player1 is not None
    
    def handle_opponent_disconnect(self, server) -> None:
        self.__logger.info(f"{self.player2.username} left {self.owner.username}'s match")

        disconn_msg = mtypes.PlayerDisonnected(self.id, False)
        self.player2 = None
        self.owner.ws_handler.send_message(disconn_msg.to_json())        

    def player_joined(self, new_player: Player, server) -> None:
        if self.player2 is not None:
            raise Exception(f"Match {self.id} was already active when player {new_player.username} tried to join.")

        self.__logger.info(f"{new_player.username} joined {self.owner.username}'s match")

        self.player2 = new_player
        self.start_game()

    def send_state_to_players(self) -> None:
        g_state = self.game.state_to_json()
        self.player1.ws_handler.send_message(g_state)
        self.player2.ws_handler.send_message(g_state)

    def start_game(self) -> None:
        self.__logger.info(f"Game at match {self.id} was started")
        self.game.reset_board()
        self.send_state_to_players()

    def handle_game_message(self, player: Player, server, msg_type, json_data: dict) -> None:
        pass