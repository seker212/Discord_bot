import asyncio
import discord
from discord.ext import commands,tasks

from lxml import html
import requests

class Timer(commands.Cog, name='Core'):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.channel = None
    
    def cog_unload(self):
        pass

    def cog_check(self, ctx: commands.Context):
        if not ctx.guild:
            raise commands.NoPrivateMessage('This command can\'t be used in DM channels.')
            #return False
        return True
    
    async def cog_command_error(self, ctx: commands.Context, error: commands.CommandError):
        await ctx.send('Timer sie wywalił '+str(error))

    @commands.command(name='timer')
    @commands.has_permissions(manage_guild=True)
    async def _timer(self, ctx):
        """Set the channel that will be used for reminder"""

        try:
            self.channel = ctx.message.channel_mentions[0]
            await ctx.send('Timer channel set to: '+self.channel.mention)
            self.schedudle.start()
        except IndexError:
            await ctx.send('Bruh you forget the channel lel')
            pass

    @tasks.loop(minutes=10.0)
    async def schedudle(self):
        """Check of a new plan"""

        if self.channel != None:
            page = requests.get('http://www.cat.put.poznan.pl/pl/dla-studentow/informatyka')
            value = html.fromstring(page.content).xpath('/html/body/div/div/div[3]/div[1]/div/article/div/div/div/table[1]/tbody/tr[6]/td[4]/em/span/span/text()')
            if value[0] != 'v25.09':
                embed = discord.Embed(title='NEW PLAN ARRIVED',description='Yes it is here',color=0xff0000)
                await self.channel.send(self.channel.guild.default_role,embed=embed)
                self.schedudle.stop()


def setup(bot):
    """Add component"""

    bot.add_cog(Timer(bot))