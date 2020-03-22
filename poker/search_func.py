from poker.deck import *

def countCards(card, cardList):
    count = 0
    if card.suit != None:
        for c in cardList:
            if card.suit == c.suit:
                count += 1
    elif card.rank != None:
        for c in cardList:
            if card.rank == c.rank:
                count += 1
    return count

def findCard(card, cardList):
    if card.suit != None and card.rank == None:
        for c in cardList:
            if card.suit == c.suit:
                return c
    elif card.rank != None and card.suit == None:
        for c in cardList:
            if card.rank == c.rank:
                return c
    elif card.suit != None and card.rank != None:
        for c in cardList:
            if card == c:
                return c
    return None

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

def flush(suit, cR, handCards):
    hand = []
    for index in range(13):
        if cR[index] > 0:
            c =findCard(Card(suit,Ranks[12-index]), handCards) 
            if c != None:
                hand.append(c)
                if len(hand) == 5:
                    return hand

def revRankIndex(rank):
    for index in range(13):
        if Ranks[12-index] == rank:
            return index

def flushSuit(cS):
    for x in range(4):
        if cS[x] >= 5:
            return x

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

def sameSuitStr(start, suit, hand):
    start = 12 - start
    for rank in range(start, start-5, -1):
        for card in hand:
            if findCard(Card(suit, Ranks[rank]), hand) == None:
                return False
    return True