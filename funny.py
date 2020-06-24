import asyncio
import discord
import re
import time
import random
from discord.ext import commands

from tables import response,facts
from music import Music

class Funny(commands.Cog):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.channel = None
        self.abcd = False
        self.status = False
        self.mus = None
    
    def cog_unload(self):
        pass

    def cog_check(self, ctx: commands.Context):
        if not ctx.guild:
            raise commands.NoPrivateMessage('This command can\'t be used in DM channels.')
    
    async def cog_command_error(self, ctx: commands.Context, error: commands.CommandError):
        await ctx.send('An error occurred: {}'.format(str(error)))

    @commands.command(name='shutdown',  aliases=['off'])
    @commands.has_permissions(manage_guild=True)
    async def _shutdown(self, ctx: commands.Context):
        """Shut down the bot"""

        await ctx.channel.send("I'm about to end my life")
        await bot.close()
        print('Bot ended his life')
        pass

    @commands.command(name='gobrrr')
    @commands.has_permissions(manage_guild=True)
    async def _gobrrr(self, ctx: commands.Context):
        """Removes music class from working bot"""

        if self.status == False:
            if self.mus == None:
                self.mus = Music(self.bot)
            self.bot.add_cog(self.mus)
            self.status = True
            await ctx.channel.send("Music enabled")
        else:
            for x in self.mus.get_commands():
                self.bot.remove_command(x)
            self.bot.remove_cog(self.mus)
            self.status = False
            await ctx.channel.send("Music Disabled")

    @commands.command(name='halp')
    async def _halp(self, ctx: commands.Context):
        """Halp command lel"""

        embed = discord.Embed(title="Help",description="this displays the help commands",color=0x00ff00)
        embed.add_field(name=".setchannel <channel>",value="set the channel to log stuff",inline=False)
        embed.add_field(name=".setabcd",value="switch between true/false to adding abcd under every image",inline=False)
        embed.add_field(name=".shutdown",value="shutdowns the bot",inline=False)
        embed.add_field(name=".gobrrr",value="hehe dx",inline=False)
        embed.add_field(name="@anyone",value="pokes a random person",inline=False)
        embed.add_field(name="@self",value="( ͡° ͜ʖ ͡°)",inline=False)
        embed.add_field(name="@Chi-chan",value="Try and find out many options",inline=False)
        await ctx.send(embed=embed)

    @commands.Cog.listener()
    async def on_voice_state_update(self ,member, before, after):
        """Voice states logger """

        if self.channel != None and not member.bot:
            async with channel.typing():
                user = member.name
                ts = '[ '+time.strftime("%Y-%m-%d %H:%M:%S", time.localtime())+' ]  '
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

    @commands.command(name='setabcd')
    @commands.has_permissions(manage_guild=True)
    async def _setabcd(self, ctx: commands.Context):
        """Set channel to log on messages"""

        if self.abcd != None:
            self.abcd = not self.abcd
        else:
            self.abcd = False
        await ctx.send('ABCD set to '+str(self.abcd))
    
    @commands.Cog.listener() 
    async def on_message(self, message):
        """On message event listener """

        #:OOF: reaction to and OOF message
        emoji = discord.utils.get(message.guild.emojis, name='OOF')
        msg = message.content.lower()
        x = re.search("^[^a-zA-Z0-9]*[o]+of[^a-zA-Z0-9]*$",msg)
        if x != None:
            await message.add_reaction(emoji)

        #@anyone losowanko magicznej liczby hehe xd
        if not message.author.bot:
            if re.search('@anyone',msg) != None:
                user = random.choice(message.guild.members)
                await message.channel.send(user.mention+' '+str(random.choice(facts)))
            elif re.search('@self',msg) != None:
                await message.channel.send(message.author.mention+' ( ͡° ͜ʖ ͡°)')

        #Hello and goodbye and other stuff
        if self.bot.user.mentioned_in(message) and not message.author.bot:
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
                    await message.channel.send(str(random.choice(response))+' '+message.author.mention)

        await self.bot.process_commands(message)

        #Check if the send attachment is an image
        if self.abcd:
            try:
                for attachments in message.attachments:
                    att = attachments.url
                    for ext in ['.jpg','.png','.jpeg','.PNG','.JPEG','.JPG']:
                        if att.endswith(ext):
                            for x in ['🇦', '🇧' , '🇨', '🇩', '🇪', '🇫']:
                                emoji = bot.get_emoji(x)
                                await message.add_reaction(x)
            except IndexError:
                pass
    
    @commands.Cog.listener() 
    async def on_reaction_add(self,reaction,user):
        """Supreme reaction reaction for abcd"""

        if self.abcd:
            for react in reaction.message.reactions:
                async for users in react.users (limit=None,after=None):
                    if react != reaction and users == user and user != bot.user:
                        await reaction.remove(user)
