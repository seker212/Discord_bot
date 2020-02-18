from random import randint
from variables import *
from deck import *
from search_func import *
from Player import *
from Hand import *
from TablePlayers import *
from pair import *

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

    #returns < a list of pair(first = player, second = Hand()) of winner(s), string on final arg> 
    def showdown(self):
        decisive = None
        same = []
        for p in self.playerList.List:
            if p.fold == False:
                current = pair(p, self.check_hand(p))
                if len(same) == 0:
                    same.append(current)
                elif current.second.HandType.value < same[0].second.HandType.value:
                    same.clear()
                    same.append(current)
                elif current.second.HandType.value == same[0].second.HandType.value:
                    same.append(current)
        if len(same) == 1:
            decisive = 'type'
            return same, decisive
        else:
            sameh = []
            for x in same:
                if len(sameh) == 0:
                    sameh.append(x)
                elif x.second.hight < winning.second.hight:
                    sameh.clear()
                    sameh.append(x)
                elif x.second.hight == winning.second.hight:
                    sameh.append(x)
            if len(sameh) == 1:
                decisive = 'hight'
                return sameh, decisive
            else:
                samek = []
                for i in range(len(sameh[0].second.Kickers)):
                    for p in sameh:
                        if len(samek) == 0:
                            samek.append(p) 
                        elif p.second.Kickers[i] < samek[0].second.Kickers[i]:
                            samek.clear()
                            samek.append(p)
                        elif p.second.Kickers[i] == samek[0].second.Kickers[i]:
                            samek.append(p)
                    if len(samek) == 1:
                        decisive = 'kicker'
                        return samek, decisive
                    else:
                        sameh = samek
                        samek.clear()
                return sameh, decisive

    def check_hand(self, player):
        # Ranks [2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K, A]
        # cR [A, K, Q, J, 10, 9, 8, 7, 6, 5, 4, 3, 2]
        #     0  1  2  3  4   5  6  7  8  9 10 11 12
        # cS [♦️, ♥️, ♣️, ♠️]
        cR = []
        cS = []
        for x in Ranks:
            cR.append(countCards(Card(None, x), self.tableCards + player.hand))
        cR.reverse()
        maxRN = max(cR)


        if maxRN == 4:
            return Hand(HandType.four_of_a_kind, cR.index(4), firstNo0(1, cR, cR.index(4)))

        if maxRN == 3 and 2 in cR:
            return Hand(HandType.full_house, cR.index(3), [cR.index(2)]*2)


        straight = has_straight(cR)
        for x in Suites:
            cS.append(countCards(Card(x, None),self.tableCards + player.hand))
        maxSN = max(cS)

        if maxSN >= 5:
            fSN = flushSuit(cS)
            if straight != None:
                if s == 0:
                    if sameSuitStr(s, Suites[fSN], self.tableCards + player.hand):
                        return Hand(HandType.royal_flash, s, None)
                else:
                    if sameSuitStr(s, Suites[fSN], self.tableCards + player.hand):
                        return Hand(HandType.straight_flash, s, None)

            hand = flush(Suites[fSN], cR, self.tableCards + player.hand)
            kickers = []
            for x in range(1,5):
                kickers.append(revRankIndex(hand[x].rank))
            return Hand(HandType.flush, revRankIndex(hand[0].rank), kickers)

        if straight != None:
            return Hand(HandType.straight, straight, None)

        if maxRN == 3:
            return Hand(HandType.three_of_a_kind, cR.index(3), firstNo0(2, cR, cR.index(3)))
        if maxRN == 2:
            if cR.count(2) >= 2:
                kickers = []
                save = None
                for x in range(cR.index(2)+1, 13):
                    if cR[x] == 2:
                        kickers.append([x]*2)
                        save = x
                        break
                for x in range(13):
                    if x != cR.index(2) and x != save and cR[x] > 0:
                        kickers.append([x])
                        break
                return Hand(HandType.two_pair,cR.index(2),kickers)
            else:
                return Hand(HandType.pair, cR.index(2), firstNo0(3, cR, cR.index(2)))
        
        high = []
        for x in range(13):
            if cR[x] > 0:
                high = x
                break
        return Hand(HandType.high_hand, high, firstNo0(4, cR))