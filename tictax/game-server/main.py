import time, os, sys, io
import traceback
from global_logger import Log

from src.utils import config
from src.game_server_manager import GameServerManager

def main():
    CONFIG = config.get_config()
    server_mngr = GameServerManager(CONFIG)
    server_mngr.start_bgw()

    while True:
        time.sleep(1)

if __name__ == '__main__':     
    glob_log = Log.get_logger(name="main_ex", logs_dir=os.path.abspath("./log"))
    try:
        main()
    except Exception as err:
        memStream = io.StringIO()
        traceback.print_exc(file=memStream)
        glob_log.error(f"{memStream.getvalue()}")