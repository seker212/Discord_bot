import enum

class Suit:
    def __init__(self, name, symbol):
        self.name = name
        self.symbol = symbol
    
    def __eq__(self, other):
        if self.name == other.name and self.symbol == other.symbol:
            return True
        else:
            return False
    
    def __ne__(self, other):
        return (not self == other)

class Card:
    def __init__(self, suit, rank):
        self.suit = suit
        self.rank = rank
    
    def __str__(self):
        return self.rank+self.suit.symbol

    def __eq__(self, other):
        return self.rank == other.rank and self.suit == other.suit

Ranks = ['2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K', 'A']
Suites = [Suit('diamonds', '♦️'), Suit('hearts', '♥️'), Suit('clubs', '♣️'), Suit('spades', '♠️')]

def deckFill():
    global Ranks
    global Suit
    L = []
    for s in Suites:
        for r in Ranks:
            L.append(Card(s,r))
    return L