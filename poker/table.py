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
        self.check = False

class TablePlayers:
    def __init__(self):
        self.List = []

    def __iter__(self):
        self.index = 0
        return self
    
    def __next__(self):
        player = self.List[self.index]
        if (self.index == len(self.List)-1):
            self.index = 0
        else:
            self.index += 1
        return player
    
    def __getitem__(self, key):
        return self.List[key]

    def __len__(self):
        return len(self.List)

    def __str__(self):
        end = '[ '
        for p in self.List:
            end += p.user + ' '
        end += ']'
        return end
    
    def append(self, element):
        return self.List.append(element)

    def setIterSB(self):
        i = 0
        for p in self.List:
            if p.blind.name == 'small':
                break
            i += 1
        if i > 0:
            self.index = i-1
        else:
            self.index = 0
                

class Table:
    def __init__(self, playerList): #TODO Exception: Check if len(playerList) > 1
        self.stage = 0

        self.playerList = TablePlayers()
        for entry in playerList:
            self.playerList.append(Player(entry))
        
        
        self.deck = deckFill()
        self.tableCards = []
        self.smallBlind = SMALL_BLIND
        self.number_of_deals = 0
        self.pot = 0
        self.table_ammount = 0

        self.playerIt = iter(self.playerList)
        next(self.playerIt).blind = Blind.small
        next(self.playerIt).blind = Blind.big
        self.getBlinds()
   
        self.turn = next(self.playerIt)
        self.giveHand()
    
    def __str__(self):
        end = 'Players: ' + str(self.playerList) + '\n'
        end += 'Number of deals: ' + str(self.number_of_deals) + '\n'
        end += 'Stage: ' + str(self.stage) + '\n'
        end += 'Cards in deck: ' + str(len(self.deck)) + '\n'
        end += 'Table cards: ' + str(*self.tableCards) + '\n'
        end += 'Small blind ammount: ' + str(self.smallBlind) + '\n'
        end += 'Pot: '+ str(self.pot) + '\n'
        end += 'Table ammount: ' + str(self.table_ammount) + '\n'
        end += 'Turn: ' + self.turn.user
        return end

    def giveHand(self):
        for x in range(2):
            for p in self.playerList.List:
                a = randint(0, len(self.deck)-1)
                card = self.deck[a]
                p.hand.append(card)
                self.deck.remove(card)

    def getBlinds(self): #side pot?
        self.playerList.setIterSB()
        
        sp = next(self.playerIt)
        
        if sp.money >= self.smallBlind:
            sp.money -= self.smallBlind
            sp.table_money += self.smallBlind
            self.pot += self.smallBlind
        else:
            sp.all_in = True
            sp.table_money += sp.money
            self.pot += sp.money
            sp.money = 0

        bp = next(self.playerIt)

        bigBlind = 2 * self.smallBlind
        if bp.money >= bigBlind:
            bp.money -= bigBlind
            bp.table_money += bigBlind
            self.pot += bigBlind
            self.table_ammount = bigBlind
        else:
            bp.all_in = True
            bp.table_money += bp.money
            self.pot += bp.money
            bp.money = 0

    def dealTable(self, cardsNum): 
        for x in range(cardsNum):
            a = randint(0, len(self.deck)-1)
            card = self.deck[a]
            self.tableCards.append(card)
            self.deck.remove(card)

    def finished_stage(self):
        if self.table_ammount != 0:
            x = 0
            for p in self.playerList.List:
                if p.fold == True or p.all_in == True:
                    x += 1
                elif p.table_money == self.table_ammount:
                    if p.blind.name == 'big':
                        if p.check == True:
                            x += 1
                        else:
                            return False
                    else:
                        x += 1
                else:
                    return False
            
            if x == len(self.playerList):
                return True
        else:
            for p in self.playerList.List:
                if p.fold == False and p.all_in == False and p.check == False:
                    return False
                else:
                    return True

    def nextTurn(self):
        self.turn = next(self.playerIt)

    def nextStage(self):                    #czy kolejność zmienia się co *rozdanie* czy co staga?
        if self.stage == 0:
            self.stage = 1
            self.dealTable(3)
        elif self.stage == 1:
            self.stage = 2
            self.dealTable(1)
        elif self.stage == 2:
            self.stage = 3
            self.dealTable(1)

        for p in self.playerList.List:
            p.table_money = 0
        
        self.playerList.setIterSB()
        next(self.playerIt) #small blind
        next(self.playerIt) #big blind
        self.turn = next(self.playerIt)

    def nextDeal(self):
        self.deck = deckFill()
        self.stage = 0
        self.pot = 0
        self.tableCards = []
        
        for p in self.playerList.List:
            p.fold = False
            p.fold = False
            p.table_money = 0

        if self.number_of_deals == BLIND_INC_STEP-1:
            self.number_of_deals = 0
            self.smallBlind += BLIND_INC
        else:
            self.number_of_deals += 1
        
        self.playerList.setIterSB()
        next(self.playerIt).blind = Blind.none #small blind
        next(self.playerIt).blind = Blind.small #big blind
        next(self.playerIt).blind = Blind.big
        self.getBlinds()

        self.turn = next(self.playerIt)
        self.giveHand()