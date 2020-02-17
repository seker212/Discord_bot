from player_actions import *

t1 = Table(['A', 'B', 'C', 'D', 'E'])
for p in t1.playerList.List:
    print(p.user + ':', *p.hand)

print(t1)
