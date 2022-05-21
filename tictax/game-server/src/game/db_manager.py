import settings
import pyodbc
import time
from src.game.player import Player

class DBManager():

    def __init__(self) -> None:
        self.conn = pyodbc.connect(settings.DB_CONN)

    def __execute_and_commit(self, query):
        self.__execute_and_commit_params(query)

    def __execute_and_commit_params(self, query, *params):
        cursor = self.conn.cursor()
        cursor.execute(query, params)
        cursor.commit()

    def create_match_ret_id(self, owner: Player) -> int:
        """ Adds new match to database table Match
        and returns last row id.
        """
        cursor = self.conn.cursor()
        cursor.execute(
            "INSERT INTO Match (CreationTime, OwnerUsername) VALUES (?,?)", 
            (int(time.time()), owner.username)
            )

        record_id = cursor.execute('SELECT @@IDENTITY AS id;').fetchone()[0]
        cursor.commit()

        return record_id

    def opponent_join(self, match_id: int, opponent: Player) -> None:
        """ Updates Match table by changing opponent username 
        to reference the joining opponent. 
        """
        self.__execute_and_commit_params(
            "UPDATE Match SET OpponentUsername=? WHERE Id=?",
            opponent.username,
            match_id
        )

    def remove_match(self, match_id: int) -> None:
        """ Removes match with specified id from 
        Match table and it won't be appearing in server browser 
        on client side
        """
        self.__execute_and_commit_params(
            "DELETE FROM Match WHERE Id=?",
            match_id
        )