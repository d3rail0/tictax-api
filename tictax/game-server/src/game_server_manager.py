from websocket_server import WebsocketServer
from global_logger import Log
import threading 
import traceback

from src.metabase import *

class WebsocketServerEx(WebsocketServer):

    def disconnect_client(self, client: dict):        
        client["handler"].send_close(1011, b'auth-fail')

class WebsocketServerData:
    def __init__(self, lhost, lport):
        self.host = lhost
        self.port = lport
        self.server = WebsocketServerEx(lport, host=lhost)

class GameServerManager(metaclass=MetaBase):

    MAX_MESSAGE_LEN = 128

    def __init__(self, config: dict) -> None:
        self.__logger: Log

        self.ws_srv_data = WebsocketServerData(config['ws_host'], int(config['ws_port']))

        # Set callbacks for server
        self.ws_srv_data.server.set_fn_new_client(self.new_client)
        self.ws_srv_data.server.set_fn_client_left(self.client_left)
        self.ws_srv_data.server.set_fn_message_received(self.message_received)

    def start_bgw(self):
        self.__logger.info("Starting websocket server...")
        self.x = threading.Thread(target=self.ws_srv_data.server.run_forever, args=(), daemon=True)
        self.x.start()
        self.__logger.info("Started!")

    # Called for every client connecting (after handshake)
    def new_client(self, client, server):
        self.__logger.info(f'New client connected and was given id {client["id"]}')
        server.send_message_to_all("Hey all, a new client has joined us")

    # Called for every client disconnecting
    def client_left(self, client, server):
        if client is None:
            return
        
        self.__logger.info(f'Client({client["id"]}) diconnected')

    # Called when a client sends a message
    def message_received(self, client, server, message):
        if len(message) > self.MAX_MESSAGE_LEN:
            self.__logger.error(f'Client({client["id"]}) sent message: {message[0:self.MAX_MESSAGE_LEN]}... [EXCEEDED LIMIT: {len(message)}]')
            return
            
        self.__logger.info(f'Client({client["id"]}) said: {message}')

        # Verify that JWT token hasn't expired
        if message[0] == '3':
            self.ws_srv_data.server.disconnect_client(client)

        # Pass the message to the MessageHandler