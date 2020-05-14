import enum
from poker.variables import *

class Blind(enum.Enum):
    none = 0
    small = 1
    big = 2

class Player:
    def __init__(self, player):
        self.user = player
        self.hand = []
        self.money = STARTING_MONEY
        self.table_money = 0
        self.blind = Blind.none
        self.fold = False
        self.all_in = False
        self.check = False
    
    def __str__(self):
        return self.user + ': ' + str(self.hand[0]) + ' ' + str(self.hand[1]) + ' ' + str(self.money) + ' ' + str(self.table_money) + ' ' + self.blind.name + ' ' + str(self.fold) + ' ' + str(self.all_in) + ' ' + str(self.check) #FIXME: USER