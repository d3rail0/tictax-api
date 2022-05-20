import json
import os

def get_config():
    
    if not os.path.exists("./config/"):
        os.mkdir("./config/")
    
    if not os.path.exists("./config/config.json"):
        def_config = {
            "ws_host": '127.0.0.1', 
            "ws_port": 13024
        }
        save_config(def_config)
        return def_config       
    
    with open(r"config/config.json", "r") as read_file:
        config = json.load(read_file)
        return config


def save_config(config):
    with open(r"config/config.json", "w+") as write_file:
        json.dump(config, write_file)