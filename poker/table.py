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
    
    def __str__(self):
       return self.user + ': ' + str(self.hand[0]) + ' ' + str(self.hand[1]) + ' ' + str(self.money) + ' ' + str(self.table_money) + ' ' + self.blind.name + ' ' + str(self.fold) + ' ' + str(self.all_in) + ' ' + str(self.check)

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
        end += 'Table cards: '
        for card in self.tableCards:
            end += str(card) + ' '
        end += '\n'
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
                    if p.blind.name == 'big' and self.table_ammount == 2 * self.smallBlind:
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
        if self.turn.fold == True or self.turn.all_in == True:
            self.nextTurn()

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
            p.check = False
        
        self.playerList.setIterSB()
        next(self.playerIt) #small blind
        next(self.playerIt) #big blind
        self.turn = next(self.playerIt)
        self.table_ammount = 0

    def nextDeal(self):
        self.deck = deckFill()
        self.stage = 0
        self.pot = 0
        self.tableCards = []
        
        for p in self.playerList.List:
            p.fold = False
            p.all_in = False
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

    def showdown(self):
        for p in self.playerList.List:
            if p.fold == False:
                res = self.check_hand(p)
                


    def check_hand(self, player):
        # [2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K, A]
        # [A, K, Q, J, 10, 9, 8, 7, 6, 5, 4, 3, 2]
        #  0  1  2  3  4   5  6  7  8  9 10 11 12
        # [♦️, ♥️, ♣️, ♠️]
        cR = []
        cS = []
        for x in Ranks:
            cR.appent(countCards(Card(None, x), self.tableCards + player.hand))
        cR.reverse()
        maxRN = max(cR)
        if maxRN == 4:
            return Hand(HandType.four_of_a_kind, cR.index(4), firstNo0(1, cR, cR.index(4)))
        if maxRN == 3 and 2 in cR:
            return Hand(HandType.full_house, cR.index(3), [cR.index(2)]*2)
        #DO TĄD OGAR
        straight = has_straight(cR)

        for x in Suites:
            cS.append(countCards(Card(x, None),self.tableCards + player.hand))
        maxSuitNum = max(cS)
        if maxSuitNum >= 5:
            pass #flush, straight flush, royal straight 
        else:
            if straight != None: #straight, high card
                return Hand(HandType.straight, straight, [])
        
        if maxRN == 3:
            return Hand(HandType.three_of_a_kind, cR.index(3), firstNo0(2, cR, cR.index(3))) # also possible straight and flush FUCK
        if maxRN == 2:
            if maxRN.count(2) >= 2:
                return Hand(HandType.two_pair,cR.index(2),firstNo0(3, cR, cR.index(2)))
            else:
                return HandType.pair

def has_straight(cR):
    i = 0
    s = None
    t = False
    for x in cR:
        if x != 0:
            i += 1
        else:
            i = 0
        if i == 1:
            s = x
        if i == 0 and t == False:
            s = None
        if x == 5:
            t = True
            return s
    return None

def firstNo0(n, lista, avoid = None):
    out = []
    i = 0
    for x in lista:
        if x != 0 and i != avoid:
            out += [i]*x
        i += 1
        if len(out) == n:
            return out
        elif len(out) > n:
            while len(out) > n:
                out.pop()
            return out

class Hand:
    def __init__(self, HandType, hight, Kickers):
        self.HandType = HandType
        self.hight = hight #the lower the better [A, K, Q, J, 10, 9, 8, 7, 6, 5, 4, 3, 2]
        self.Kickers = []
        self.Kickers += Kickers

class HandType(enum.Enum):
    royal_flash = 1
    straight_flash = 2
    four_of_a_kind = 3
    full_house = 4
    flush = 5
    straight = 6
    three_of_a_kind = 7
    two_pair = 8
    pair = 9
    high_hand = 10