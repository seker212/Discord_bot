import asyncio
import discord
import re

import time
import datetime
import pytz

import random
import os.path
from discord.ext import commands

from tables import response,facts

class Stuff(commands.Cog):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.channel = None
        self.voice = None
        self.timezone = None
    
    def cog_unload(self):
        pass

    def cog_check(self, ctx: commands.Context):
        if not ctx.guild:
            raise commands.NoPrivateMessage('This command can\'t be used in DM channels.')

        return True
    
    async def cog_command_error(self, ctx: commands.Context, error: commands.CommandError):
        await ctx.send(ctx.author.mention+" Jeb sie | "+ str(error))

    #---Commands definitions---
    @commands.command(name='shutdown',  aliases=['off'])
    @commands.has_permissions(manage_guild=True)
    async def _shutdown(self, ctx: commands.Context):
        """Shut down the bot"""

        await ctx.channel.send("I'm about to end my life")
        await self.bot.close()
        print('Bot ended his life')

    @commands.command(name='halp')
    async def _halp(self, ctx: commands.Context):
        """Halp command lel"""

        embed = discord.Embed(title="Help",description="this displays the help commands",color=0x00ff00)
        embed.add_field(name=".setchannel <channel>",value="set the channel to log stuff",inline=False)
        embed.add_field(name=".shutdown",value="shutdowns the bot",inline=False)
        embed.add_field(name="@anyone",value="pokes a random person",inline=False)
        embed.add_field(name="@self",value="( ͡° ͜ʖ ͡°)",inline=False)
        embed.add_field(name="@Chi-chan",value="Try and find out many options",inline=False)
        await ctx.send(embed=embed)

    @commands.command(name='setchannel')
    @commands.has_permissions(manage_guild=True)
    async def _setchannel(self, ctx: commands.Context):
        """Set channel to log on messages"""

        try:
            self.channel = ctx.message.channel_mentions[0]
            await ctx.send('Log channel set to: '+self.channel.mention)
        except IndexError:
            await ctx.send('Bruh you forget the channel lel')
            pass

    @commands.command(name='settimezone')
    @commands.has_permissions(manage_guild=True)
    async def _settimezone(self, ctx: commands.Context, zone):
        """Set the bot working timezone"""

        self.timezone = pytz.timezone(zone)
        await ctx.send('Time zone set to: ' + str(self.timezone))
    
    #---Listener section---#
    @commands.Cog.listener()
    async def on_voice_state_update(self ,member, before, after):
        """Voice states logger """

        if self.channel != None and not member.bot:
            async with self.channel.typing():
                user = member.name
                ts = '[ '+ datetime.datetime.now(self.timezone).strftime("%Y-%m-%d %H:%M:%S")+' ]  '
                if after.channel == None:
                    await self.channel.send(ts+'***'+user+'*** left voice channel ***'+before.channel.mention+'***')
                elif before.channel == None:
                    await self.channel.send(ts+'***'+user+'*** joined voice channel ***'+after.channel.mention+'***')
                elif after.channel != before.channel:
                    await self.channel.send(ts+'***'+user+'*** changed voice channel ***'+before.channel.mention+'*** to ***'+after.channel.mention+'***')
                elif after.deaf != before.deaf:
                    if after.deaf:
                        await self.channel.send(ts+'***'+user+'*** fully muted by guild')
                    else:
                        await self.channel.send(ts+'***'+user+'*** fully unmuted by guild')
                elif after.mute != before.mute:
                    if after.mute:
                        await self.channel.send(ts+'***'+user+'*** muted by guild')
                    else:
                        await self.channel.send(ts+'***'+user+'*** unmuted by guild')   
                elif after.self_deaf != before.self_deaf:
                    if after.self_deaf:
                        await self.channel.send(ts+'***'+user+'*** fully muted by himself')
                    else:
                        await self.channel.send(ts+'***'+user+'*** fully unmuted by himself')
                elif after.self_mute != before.self_mute:
                    if after.self_mute:
                        await self.channel.send(ts+'***'+user+'*** muted by himself')
                    else: 
                        await self.channel.send(ts+'***'+user+'*** unmuted by himself')
                elif after.self_stream != before.self_stream:
                    if after.self_stream:
                        await self.channel.send(ts+'***'+user+'*** started streaming')
                    else: 
                        await self.channel.send(ts+'***'+user+'*** stopped streaming')
                elif after.self_video != before.self_video:
                    if after.self_video:
                        await self.channel.send(ts+'***'+user+'*** shows himself in video call')
                    else:
                        await self.channel.send(ts+'***'+user+'*** hides himself in video call')
                
        #No afk channel policy
        if after.channel != None:
            if after.channel == after.channel.guild.afk_channel:
                await member.move_to(None,reason="OOF")
    
    @commands.Cog.listener() 
    async def on_message(self, message):
        """On message event listener """

        if not message.author.bot:
            #:OOF: reaction to and OOF message
            emoji = discord.utils.get(message.guild.emojis, name='OOF')
            msg = message.content.lower()
            x = re.search("^[^a-zA-Z0-9]*[o]+of[^a-zA-Z0-9]*$",msg)
            if x != None and emoji != None:
                await message.add_reaction(emoji)

            #@anyone losowanko magicznej liczby hehe xd
            if re.search('@anyone',msg) != None:
                user = random.choice(message.guild.members)
                await message.channel.send(user.mention+' '+str(random.choice(facts)))
            elif re.search('@self',msg) != None:
                await message.channel.send(message.author.mention+' ( ͡° ͜ʖ ͡°)')

            #Hello and goodbye and other stuff
            if self.bot.user.mentioned_in(message):
                async with message.channel.typing():
                    if re.search('((dzie([ń|n]) dobry)|(cze([s|ś][c|ć]))) .*', msg) != None:
                        await message.channel.send('Dzień dobry! '+message.author.mention)
                    elif re.search('dobranoc .*', msg) != None:
                        await message.channel.send('Dobranoc! '+message.author.mention)
                    elif re.search('wypierdalaj .*', msg) != None:
                        await message.channel.send('Sam wypierdalaj '+message.author.mention)
                    elif re.search('[\?] .*', msg) != None or re.search('.* [\?]',msg):
                        emoji = discord.utils.get(message.guild.emojis, name='kannahm')
                        await message.add_reaction(emoji)
                    else:
                        #Tak tutaj napewnie nie dzieją sie dziwne rzeczy
                        if self.bot.user.id in message.raw_mentions:
                            await message.channel.send(str(random.choice(response))+' '+message.author.mention)

        else:
            await self.bot.process_commands(message)

    @commands.Cog.listener()
    async def on_member_update(self,before, after):
        """Supreme status stalking"""
        #TODO: Fix me when you add SQLite or some other DB
        """
        if not before.bot and self.channel != None:
            user = before.name
            stamp = '[ '+time.strftime("%Y-%m-%d %H:%M:%S", time.localtime())+' ]  '
            if before.activity != after.activity:
                if before.activity == None:
                    await channel.send(stamp+'***'+user +'*** Started playing: '+str(after.activity.name))
                elif after.activity == None:
                    played = datetime.datetime.utcnow() - before.activity.start
                    await channel.send(stamp+'***'+user +'*** Stopped playing: '+str(before.activity.name)+' for '+str(played))
                else:
                    if before.activity.name != 'Spotify' or before.activity.name != 'Spotify':
                        played = after.activity.start - before.activity.start
                        await channel.send(stamp+'***'+user +'*** Changed status: '+str(before.activity.name)+' to '+str(after.activity.name)+' for '+str(played))
        """

def setup(bot):
    """Add component"""

    bot.add_cog(Stuff(bot))
