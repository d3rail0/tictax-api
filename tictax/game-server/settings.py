# settings.py
import os
from dotenv import load_dotenv, find_dotenv

load_dotenv(find_dotenv())

TICTAX_JWT_KEY = os.environ.get("TICTAX_JWT_KEY")
DB_CONN = os.environ.get("DB_CONN")