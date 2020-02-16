from random import randint
from variables import *
from deck import *

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


class Table:
    def __init__(self, playerList): #TODO Exception: Check if len(playerList) > 1
        self.stage = 0

        self.playerList = []
        for entry in playerList:
            self.playerList.append(Player(entry))
        
        self.deck = deckFill()
        
        if len(playerList) > 1:
            self.turnPI = 2
        else:
            self.turnPI = 0
        self.turn = self.playerList[self.turnPI]
        
        self.smallBlind = SMALL_BLIND
        #self.bigBlind = 2*self.smallBlind
        self.pot = 3 * self.smallBlind
        self.table_ammount = 2 * self.smallBlind
        self.number_of_deals = 0

        self.smallBlindPlayerIndex = 0
        self.playerList[self.smallBlindPlayerIndex].blind = Blind.small
        self.playerList[self.smallBlindPlayerIndex + 1].blind = Blind.big        
        self.giveHand()
    
    def giveHand(self):
        for x in range(2):
            for p in self.playerList:
                a = randint(0, len(self.deck)-1)
                card = self.deck[a]
                p.hand.append(card)
                self.deck.remove(card)

    def getBlinds(self):
        sp = self.playerList[self.smallBlindPlayerIndex]
        sp.blind = Blind.small
        if sp.money >= self.smallBlind:
            sp.money -= self.smallBlind
            sp.table_money += self.smallBlind
            self.pot += self.smallBlind
        else:
            sp.all_in = True
            sp.table_money += sp.money
            self.pot += sp.money
            sp.money = 0

        if self.smallBlindPlayerIndex == len(self.playerList)-1:
            bp = self.playerList[0]
        else:
            bp = self.playerList[self.smallBlindPlayerIndex + 1]
        bp.blind = Blind.big
        if bp.money >= 2 * self.smallBlind:
            bp.money -= 2 * self.smallBlind
            bp.table_money += 2 * self.smallBlind
            self.pot += 2 * self.smallBlind
        else:
            bp.all_in = True
            bp.table_money += bp.money
            self.pot += bp.money
            bp.money = 0
        
    def nextTurn(self):
        pass

    def nextStage(self):
        pass

    def nextDeal(self):
        self.deck = deckFill()
        self.giveHand()
        self.stage = 0
        
        if self.smallBlindPlayerIndex == len(self.playerList)-1:
            self.smallBlindPlayerIndex = 0
        else:
            self.smallBlindPlayerIndex += 1
        self.getBlinds()

        for p in self.playerList:
            p.fold = False
            p.fold = False
            p.table_money = 0

        if self.number_of_deals == 4:
            self.number_of_deals = 0
            self.smallBlind += BLIND_INC
        else:
            self.number_of_deals += 1

        #TODO Unfinished