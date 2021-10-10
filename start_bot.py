from os import getenv
from sys import argv
from core import *

import logger as log
logger = log.getLogger(__name__)

token = getenv('TOKEN')
if token is None:
    if len(argv) == 1:
        with open('settings/token.txt', 'r') as token_file:
            token = token_file.read()
    elif len(argv) == 2:
        token = argv[1]
    else:
        logger.error("ArgumentError: Too many arguments")
if token is not None:
    logger.info("Starting bot ...")
    bot.run(token)