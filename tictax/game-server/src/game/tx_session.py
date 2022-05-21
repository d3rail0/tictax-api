from zoneinfo import available_timezones
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

    def __randomize_symbols(self) -> None:
        rand_num = randrange(0, 2)

        self.player1_symbol = self.symbols[rand_num]
        self.player2_symbol = self.symbols[(rand_num+1) % 2]

        self.p_symbols: dict[str, str] = {
            self.player1_symbol: self.player1.username,
            self.player2_symbol: self.player2.username
        }

    def __reset_score(self) -> None:
        self.p_scores: dict[str, str] = {
            self.player1.username : 0,
            self.player2.username : 0
        }

    def is_active(self) -> bool:
        """ Returns true if two players had already joined
        the same session.
        """
        return self.player2 is not None and self.player1 is not None
    
    def handle_opponent_disconnect(self, server) -> None:
        self.__logger.info(f"{self.player2.username} left {self.owner.username}'s match")

        disconn_msg = mtypes.PlayerDisonnected(self.id, False)
        self.player2: Player = None
        self.owner.ws_handler.send_message(disconn_msg.to_json())        

    def player_joined(self, new_player: Player, server) -> None:
        if self.player2 is not None:
            raise Exception(f"Match {self.id} was already active when player {new_player.username} tried to join.")

        if new_player.username == self.owner.username:
            new_player.send_error("Cannot play against yourself!")
            raise Exception(f"{new_player.username} tried to play against themself")

        self.__logger.info(f"{new_player.username} joined {self.owner.username}'s match")

        self.player2: Player = new_player
        # Reset score when new opponent joins
        self.__reset_score()
        self.__randomize_symbols()
        self.start_game()

    def send_state_to_players(self) -> None:
        g_state = self.game.get_state()

        g_state.opponent = self.player2.username
        self.player1.ws_handler.send_message(g_state.to_json())
        g_state.opponent = self.player1.username
        self.player2.ws_handler.send_message(g_state.to_json())

    def send_message_to_all(self, message) -> None:
        if self.player1 is not None:
            self.player1.send_message(message)
        
        if self.player2 is not None:
            self.player2.send_message(message)

    def send_scores_to_players(self) -> None:
        pass

    def start_game(self) -> None:
        self.__logger.info(f"Game at match {self.id} was started")
        self.game.reset_board()    
        self.send_state_to_players()

    def handle_game_message(self, player: Player, server, msg_type, json_data: dict) -> None:
        
        if msg_type != mtypes.PLAY_MOVE:
            raise Exception('Unrecognized game message')
        
        p_move: mtypes.PlayMove = mtypes.apply_schema_conv(mtypes.PlayMove, json_data)

        is_valid_move = False
        
        if player.client_id == self.owner.client_id:
            is_valid_move = self.game.play_move(p_move.cell, self.player1_symbol)
        else:
            is_valid_move = self.game.play_move(p_move.cell, self.player2_symbol)

        if not is_valid_move:
            player.send_error(f'Invalid move')
            return
        
        # Update clients with new state
        self.send_state_to_players()

        # Check for winners or a draw
        winner_symbol: str = self.game.get_winner()

        if winner_symbol != '':
            # Somebody won
            self.p_scores[self.p_symbols[winner_symbol]]+=1

            game_end_msg = mtypes.GameEnd(
                self.id, 
                self.p_symbols[winner_symbol], 
                False, 
                self.p_scores
            )
            
            self.send_message_to_all(game_end_msg.to_json())

        elif len(self.game.available_moves()) == 0:
            # No more moves, it's a TIE!
            game_end_msg = mtypes.GameEnd(
                self.id, 
                "", 
                True,
                self.p_scores
            )

            self.send_message_to_all(game_end_msg.to_json())


