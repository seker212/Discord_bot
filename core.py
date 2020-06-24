import asyncio
import discord
import pickle
from discord.ext import commands
#from poker.discord_control import *
from poker.pair import *

from funny import Funny

bot = commands.Bot(command_prefix= '.')
core = Core(bot)
bot.add_cog(Core)

class Core(commands.Cog):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.voiceBot = None
        self.audiofile = None
        self.games = []
        self.fun = Funny(self.bot)
    
    def cog_unload(self):
        pass
    
    @commands.Cog.listener()
    async def on_ready(self):
        """On ready command list"""

        print('Logged in as {0.user}'.format(self.bot))
        await self.bot.change_presence(activity=discord.Game(name='WEEEEEEEEEEEEEEEEEEEEEEEEEEE'))
        self.bot.add_cog(fun)
        #load()

    @commands.command(name='on')
    async def _on(self,ctx):
        """Try if it is alive"""

        emoji = discord.utils.get(ctx.guild.emojis, name='nyan')
        if emoji:
            await ctx.send(ctx.author.mention+" I'm alive "+emoji)

    @commands.command(name='cls')
    @commands.has_permissions(manage_guild=True)
    async def _cls(self,ctx):
        """Should clear chat"""

        if ctx.channel.type.name == 'text':
            await ctx.channel.purge()

    @commands.command(name='saytts')
    @commands.has_permissions(manage_guild=True)
    async def _saytts(self,ctx, channelName, message):
        """It should say it in tts"""

        if ctx.channel.type.name == 'text':
            for channel in ctx.guild.channels:
                if channel.name == channelName:
                    await channel.send(content = message, tts = True)
                    break

    @commands.command(name='say')
    @commands.has_permissions(manage_guild=True)
    async def _say(self,ctx, channelName, message):
        """It should say just it"""

        if ctx.channel.type.name == 'text':
            for channel in ctx.guild.channels:
                if channel.name == channelName:
                    await channel.send(content = message)
                    break


""" def save():
    global games
    with open('games.pkl', 'wb') as output:
        pickle.dump(games, output)

def load():
    with open('games.pkl', 'rb') as input:
        games = pickle.load(input)
 """
    
""" 
@bot.event
async def on_voice_state_update(member, before, after):
    global voiceBot
    if not member.bot:
        if after.channel != before.channel: #only channel change
            if after.channel != None:       #someone on a channel
                if after.channel.name == 'Klocki' and len(after.channel.members) == 1:
                    if len(bot.voice_clients) == 0:
                        voiceBot = await after.channel.connect()
                        await asyncio.sleep(0.5)
                        file = open('raw audio/Get_out_minecraft.raw', 'rb')
                        voiceBot.play(discord.PCMAudio(file), after = voiceBot.stop())
                        while voiceBot.is_playing():
                            await asyncio.sleep(1)
                        file.close()
                        await voiceBot.disconnect() """

"""@bot.command()
async def join(ctx, channelName):
    global voiceBot
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        for channel in ctx.guild.channels:
            if channel.name == channelName:
                voiceBot = await channel.connect()
                break

 @bot.command()
async def play(ctx, *filename):
    global voiceBot
    global audiofile
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        if len(filename) == 1:
            x = 'raw audio/{}.raw'.format("".join(filename))
            audiofile = open(x, 'rb')
            voiceBot.play(discord.PCMAudio(audiofile), after = voiceBot.stop())
            while voiceBot.is_playing():
                await asyncio.sleep(1)
        elif len(filename) == 0:
            voiceBot.play(discord.PCMAudio(audiofile), after = voiceBot.stop())
            while voiceBot.is_playing():
                await asyncio.sleep(1)

@bot.command()
async def leave(ctx):
    global audiofile
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        if voiceBot.is_connected():
            if voiceBot.is_playing():
                voiceBot.stop()
        await voiceBot.disconnect()

@bot.command()
async def stop(ctx):
    global audiofile
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        if voiceBot.is_connected() and voiceBot.is_playing():
            voiceBot.stop()
        audiofile.close()

@bot.command()
async def pause(ctx):
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        if voiceBot.is_connected() and voiceBot.is_playing():
            voiceBot.pause() """


"""@bot.command()
async def fprop(ctx, *args):
    file = open(r'function_propositions.txt', 'a')
    file.write(str(ctx.author) + ':\t' + ' '.join(args) + '\n')
    file.close()
"""

"""@bot.command()
async def game(ctx, oper, arg):
    global games
    arg = arg.lower()
    if oper == "new" and ctx.channel.name == 'bot-mod':
        for g in games:
            if arg == g.first:
                await ctx.send("I know this game already. It's " + g.first)
                return

        members = []
        games.append(pair(arg, members))
        await ctx.send('OK')

    elif oper == 'addto':
        for g in games:
            if arg == g.first:
                for m in g.second:
                    if ctx.author == m:
                        await ctx.send("You are already on the list")
                        return
                g.second.append(ctx.author)
                with open('games.pkl', 'wb') as output:
                    pickle.dump(games, output)
                await ctx.send(":)")
                return
        
        await ctx.send("I don't know what game do you mean")
                
    elif oper == 'play':
        for g in games:
            if arg == g.first:
                people = ""
                for m in g.second:
                    people += m.mention
                    people += " "
                await ctx.send("Wanna play "+g.first+" "+people)
    
    elif oper == 'rmme':
        for g in games:
            if arg == g.first:
                for m in g.second:
                    if ctx.author == m:
                        g.second.remove(m)
                        with open('games.pkl', 'wb') as output:
                            pickle.dump(games, output)
                        await ctx.send(":)")
                        return
                await ctx.send("Ups, haven't found you")
                return
        
        await ctx.send("I don't know what game do you mean")

    
    with open('games.pkl', 'wb') as output:
        pickle.dump(games, output)"""