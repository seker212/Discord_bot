import asyncio
import discord
from discord.ext import commands
#from poker.discord_control import *
#from poker.pair import *

class Core(commands.Cog, name='Core'):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.voiceBot = None
        self.audiofile = None
        self.games = []
    
    def cog_unload(self):
        pass

    def cog_check(self, ctx: commands.Context):
        if not ctx.guild:
            raise commands.NoPrivateMessage('This command can\'t be used in DM channels.')
            #return False

        return True
    
    async def cog_command_error(self, ctx: commands.Context, error: commands.CommandError):
        await ctx.send('Drukarka się psuje? '+ str(error))

    @commands.command(name='on')
    async def _on(self,ctx):
        """Try if it is alive"""

        emoji = discord.utils.get(ctx.guild.emojis, name='nyan')
        if emoji:
            await ctx.send(ctx.author.mention+" I'm alive "+str(emoji))

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

        try:
            channel = ctx.message.channel_mentions[0]
            await channel.send(content = message, tts = True)
        except IndexError:
            await ctx.send('Something went wrong')
            pass

    @commands.command(name='say')
    @commands.has_permissions(manage_guild=True)
    async def _say(self,ctx, channelName, message):
        """It should say just it"""

        try:
            channel = ctx.message.channel_mentions[0]
            await channel.send(content = message)
        except IndexError:
            await ctx.send('Something went wrong')
            pass

bot = commands.Bot(command_prefix= '.')
bot.load_extension('funny')
bot.load_extension('timer')
bot.add_cog(Core(bot))

@bot.event
async def on_ready():
    """On ready command list"""

    print('Logged in as {0.user}'.format(bot))
    await bot.change_presence(activity=discord.Game(name='WEEEEEEEEEEEEEEEEEEEEEEEEEEE'))
    #load()


