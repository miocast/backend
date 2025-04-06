import os

from dotenv import load_dotenv

load_dotenv(override=True)

DEBUG = os.environ.get('DEBUG')

MAIN_API = os.environ.get('MAIN_API')
LLM_TOKEN = os.environ.get('LLM_TOKEN')