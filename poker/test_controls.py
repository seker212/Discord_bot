from player_actions import *

def cmd(table):
    a = input()
    if a == 'call':
        return call(table)
    elif a == 'bet':
        b = int(input())
        return bet(table, b)
    elif a == 'raise':
        b = int(input())
        return Raise(table, b)
    elif a == 'fold':
        return fold(table)
    elif a == 'all_in':
        return all_in(table)
    elif a == 'check':
        return check(table)
    else:
        return ActionEffect.Input_error

def pp(t1):
    for x in range(len(t1.playerList.List)):
        print(t1.playerList.List[x])

def info(t1):
    pp(t1)
    print()
    print(t1)
    print('\n')

t1 = Table(['A', 'B', 'C', 'D'])


for deal in range(3):

    for x in range(4):
        info(t1)
        while not t1.finished_stage():
            c = cmd(t1)
            print(c.name)
            print()
            if c.name == 'OK':
                if t1.finished_stage():
                    print('but why')
                    break
                else:
                    print('shit')

                t1.nextTurn()
                info(t1)
        print('-------------------------')
        if t1.stage == 3:
            print('SHOWDOWN:')
            winners, decisive = t1.showdown()
            if len(winners) == 1:
                print('Winner: ' + winners[0].first.user + ' with ' + winners[0].second.HandType.name, end='')
                if decisive != None:
                    if decisive == 'hight' or decisive == 'kicker':
                        print(' by a ' + decisive, end ='')
            else:
                print('We have a tie between players: ', end='')
                for x in winners:
                    print(x.first.user + ' ', end='')
                print('and the pot is split')
            print('.\n', end='')
            t1.grab_pot(winners)
        else:
            t1.nextStage()
    t1.nextDeal()
    

