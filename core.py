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
        archive_po_allow = discord.Permissions(read_messages = True, read_message_history = True)
        archive_po_deny = discord.Permissions.text()
        archive_po_deny.update(read_messages = False, read_message_history = False, create_instant_invite = True)
        self.ARCHIVE_PO = discord.PermissionOverwrite.from_pair(archive_po_allow, archive_po_deny)
    
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
    async def _saytts(self,ctx, channelName, *args):
        """It should say it in tts"""
        try:
            if(len(args) > 0):
                channel = ctx.message.channel_mentions[0]
                string = " ".join(args)
                await channel.send(content = string, tts = True)
            else:
                emoji = discord.utils.get(ctx.guild.emojis, name='kannakms')
                await ctx.send("A może dałbyś mi coś do powiedzenia? " + str(emoji))
        except IndexError:
            await ctx.send('Something went wrong')
            pass

    @commands.command(name='say')
    @commands.has_permissions(manage_guild=True)
    async def _say(self,ctx, channelName, *args):
        """It should say just it"""
        try:
            if(len(args) > 0):
                channel = ctx.message.channel_mentions[0]
                string = " ".join(args)
                await channel.send(content = string)
            else:
                emoji = discord.utils.get(ctx.guild.emojis, name='kannakms')
                await ctx.send("A może dałbyś mi coś do powiedzenia? " + str(emoji))
        except IndexError:
            await ctx.send('Something went wrong')
            pass

    @commands.command(name='archive-whitelist')
    @commands.has_permissions(manage_guild=True)
    async def _archive(self,ctx):
        """Function for archiving text channels with whitelist"""

        archive_category = next(x for x in ctx.guild.categories if x.name == 'Archiwum X')

        for channel in ctx.message.channel_mentions:
            if ctx.guild.default_role not in channel.overwrites or channel.overwrites[ctx.guild.default_role].read_messages != False:
                await ctx.send(channel.mention + ' is not a whitelist channel')
                break
            for overwrite in channel.overwrites:
                if isinstance(overwrite, discord.Role):
                    if (not overwrite.is_default() and not overwrite.is_bot_managed()):
                        if channel.overwrites[overwrite].read_messages:
                            for member in overwrite.members:
                                await channel.set_permissions(member, overwrite = self.ARCHIVE_PO)
                        await channel.set_permissions(overwrite, overwrite = None)
                if isinstance(overwrite, discord.Member):
                    if channel.overwrites[overwrite].read_messages and not overwrite.bot:
                        await channel.set_permissions(overwrite, overwrite = self.ARCHIVE_PO)
            await channel.send("This channel has been archived. Accounts that had access to it at the time of archiving have read-only access.")
            await channel.edit(category = archive_category)

    @commands.command(name='here')
    async def _here(self,ctx):
        """Mentions everyone form channel with whitelist omiting admins"""

        outMessage = ''

        if ctx.guild.default_role not in ctx.channel.overwrites or ctx.channel.overwrites[ctx.guild.default_role].read_messages != False:
            await ctx.send(ctx.channel.mention + ' is not a whitelist channel')
        else:
            for overwrite in ctx.channel.overwrites:
                if isinstance(overwrite, discord.Role):
                    if (not overwrite.is_default() and not overwrite.is_bot_managed()):
                        if ctx.channel.overwrites[overwrite].read_messages:
                            outMessage += overwrite.mention + ' '
                if isinstance(overwrite, discord.Member):
                    if ctx.channel.overwrites[overwrite].read_messages and not overwrite.bot:
                        outMessage += overwrite.mention + ' '
            if outMessage:
                await ctx.message.reply(outMessage)

intents = discord.Intents.all()
bot = commands.Bot(command_prefix= '.', intents=intents)
bot.load_extension('stuff')
bot.load_extension('sounds')
bot.load_extension('ytplayer')
# bot.load_extension('timer')
bot.add_cog(Core(bot))

@bot.event
async def on_ready():
    """On ready command list"""

    print('Logged in as {0.user}'.format(bot))
    await bot.change_presence(activity=discord.Game(name='WEEEEEEEEEEEEEEEEEEEEEEEEEEE'))
    #load()


