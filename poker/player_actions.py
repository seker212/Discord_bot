from poker.Table import *

class ActionEffect(enum.Enum):
    OK = 0
    Not_enought_money = 1
    All_in_requred = 2
    Pot_already_open = 3
    Check_required = 4
    Arg_out_of_range = 5
    Too_little_ammount = 6
    Pot_is_close = 7
    Input_error = 8


def fold(table): 
    table.turn.fold = True
    return ActionEffect.OK

def bet(table, ammount):
    if ammount <= 0:
        return ActionEffect.Arg_out_of_range
    else:
        if ammount < 2*table.smallBlind:
            return ActionEffect.Too_little_ammount
        else:
            if table.turn.money < ammount:
                return ActionEffect.Not_enought_money
            else:
                if table.table_ammount == 0:
                    table.table_ammount = ammount
                    table.pot += ammount
                    table.turn.money -= ammount
                    table.turn.table_money += ammount
                    return ActionEffect.OK
                else:
                    return ActionEffect.Pot_already_open

def call(table): 
    to_call = table.table_ammount - table.turn.table_money
    if to_call == 0:
        return ActionEffect.Check_required
    elif table.turn.money <= to_call:
        return ActionEffect.All_in_requred
    elif to_call > 0:
        table.turn.money -= to_call
        table.turn.table_money += to_call
        table.pot += to_call
        return ActionEffect.OK


def all_in(table):
    table.turn.all_in = True
    to_call = table.table_ammount - table.turn.table_money
    if table.turn.money > to_call:
        table.table_ammount = table.turn.table_money + table.turn.money
    table.pot += table.turn.money
    table.turn.table_money += table.turn.money
    table.turn.money = 0
    return ActionEffect.OK

def check(table):
    if table.table_ammount == 0:
        table.turn.check = True
        return ActionEffect.OK
    else:
        if table.turn.blind.name == 'big' and table.table_ammount == 2 * table.smallBlind:
            table.turn.check = True
            return ActionEffect.OK
        return ActionEffect.Pot_already_open

def Raise(table, ammount):
    to_call = table.table_ammount - table.turn.table_money
    raise_amm = ammount - to_call
    if table.table_ammount == 0:
        return ActionEffect.Pot_is_close
    elif ammount < 0:
        return ActionEffect.Arg_out_of_range
    else:
        if table.turn.money < ammount:
            return ActionEffect.Not_enought_money
        else:
            if raise_amm < table.table_ammount:
                return ActionEffect.Too_little_ammount
            else:
                table.table_ammount += raise_amm
                table.turn.money -= ammount
                table.pot += ammount
                table.turn.table_money += ammount
                return ActionEffect.OK