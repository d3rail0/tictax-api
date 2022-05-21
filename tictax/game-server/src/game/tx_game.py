from src.metabase import MetaBase
from global_logger import Log
from typing import Tuple
import numpy as np

import src.game.protocol.msg_types as mtypes

class TxGame(metaclass=MetaBase):
    """ Implements TicTacToe game logic
    """

    def __init__(self, match_id: int) -> None:
        self.__logger: Log
        self.match_id = match_id
        self.reset_board()
        
    def reset_board(self):
        # Used to ensure that player doesn't play
        # two or more moves in a row 
        self.last_move_played: str = None

        # 3x3 matrix
        self.board: list[list[str]] = [['', '', ''], ['', '', ''], ['', '', '']]
        
        # integer mapped to a cell in board
        self.cells: dict[int, Tuple(int, int)] =  {
            # (y, x)
            1 : (0, 0),
            2 : (0, 1),
            3 : (0, 2),
            4 : (1, 0),
            5 : (1, 1),
            6 : (1, 2),
            7 : (2, 0),
            8 : (2, 1),
            9 : (2, 2)
        }

    def get_state(self) -> mtypes.GameState:
        game_state = mtypes.GameState(self)
        return game_state

    def available_moves(self) -> dict[int]:
        return self.cells.keys()

    def play_move(self, cell: int, symbol: str) -> bool:
        """ Updates specified cell on board with
        the symbol.

        Returns true if move was successful and false otherwise.
        """
        if self.last_move_played == symbol:
            return False

        if cell not in self.available_moves():
            return False

        i, j = self.cells[cell]
        self.board[i][j] = symbol
        self.cells.pop(cell)
        self.last_move_played = symbol

        return True

    def __check_rows(self):
        for row in self.board:
            if len(set(row)) == 1:
                return row[0]
        return ''

    def __check_diagonals(self):
        if len(set([self.board[i][i] for i in range(len(self.board))])) == 1:
            return self.board[0][0]
        if len(set([self.board[i][len(self.board)-i-1] for i in range(len(self.board))])) == 1:
            return self.board[0][len(self.board)-1]
        return ''

    def get_winner(self):
        for newBoard in [self.board, np.transpose(self.board)]:
            result = self.__check_rows()
            if result:
                return result
        return self.__check_diagonals()