from os import getenv
from sys import argv
from core import *

token = getenv('TOKEN')
if token is None:
    if len(argv) == 1:
        with open('token.txt', 'r') as token_file:
            token = token_file.read()
    elif len(argv) == 2:
        token = argv[1]
    else:
        print("ArgumentError: Too many arguments")
if token is not None:
    bot.run(token)