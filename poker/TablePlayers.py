class TablePlayers:
    def __init__(self):
        self.List = []
        self.saveIndex = None

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
            end += p.user + ' '                 #FIXME: USER
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
        if i == len(self.List):
            self.index = self.saveIndex
        else:
            self.index = i
            self.saveIndex = i