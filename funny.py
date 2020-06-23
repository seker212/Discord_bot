import asyncio
import discord
import re
import time
import random
from discord.ext import commands
from core import bot

from tables import response
from tables import facts

from music import Music

channel = None
abcd = False
status = False

@bot.command()
async def shutdown(ctx):
    if ctx.channel.name == 'bot-mod':
        await ctx.channel.send("I'm about to end my life")
        await bot.close()
        print('Bot ended his life')

@bot.command()
async def gobrrr(ctx):
    global status
    if ctx.channel.name == 'bot-mod':
        if status == False:
            bot.add_cog(Music(bot))
            status = True
            await ctx.channel.send("Music enabled")
        else:
            bot.remove_cog(Music(bot))
            status = False
            await ctx.channel.send("Music Disabled")

#help
@bot.command()
async def halp(ctx):
    embed = discord.Embed(title="Help",description="this displays the help commands",color=0x00ff00)
    embed.add_field(name=".setchannel <channel>",value="set the channel to log stuff",inline=False)
    embed.add_field(name=".setabcd",value="switch between true/false to adding abcd under every image",inline=False)
    embed.add_field(name=".shutdown",value="shutdowns the bot",inline=False)
    embed.add_field(name=".gobrrr",value="hehe dx",inline=False)
    embed.add_field(name="@anyone",value="pokes a random person",inline=False)
    embed.add_field(name="@self",value="( ͡° ͜ʖ ͡°)",inline=False)
    embed.add_field(name="@Chi-chan",value="Try and find out many options",inline=False)
    await ctx.send(embed=embed)

#voice channel stalking
@bot.event
async def on_voice_state_update(member, before, after):
    global channel
    if channel != None and not member.bot:
        async with channel.typing():
            user = member.name
            ts = '[ '+time.strftime("%Y-%m-%d %H:%M:%S", time.localtime())+' ]  '
            if after.channel == None:
                await channel.send(ts+'***'+user+'*** left voice channel ***'+before.channel.mention+'***')
            elif before.channel == None:
                await channel.send(ts+'***'+user+'*** joined voice channel ***'+after.channel.mention+'***')
            elif after.channel != before.channel:
                await channel.send(ts+'***'+user+'*** changed voice channel ***'+before.channel.mention+'*** to ***'+after.channel.mention+'***')
            elif after.deaf != before.deaf:
                if after.deaf:
                    await channel.send(ts+'***'+user+'*** fully muted by guild')
                else:
                    await channel.send(ts+'***'+user+'*** fully unmuted by guild')
            elif after.mute != before.mute:
                if after.mute:
                    await channel.send(ts+'***'+user+'*** muted by guild')
                else:
                    await channel.send(ts+'***'+user+'*** unmuted by guild')   
            elif after.self_deaf != before.self_deaf:
                if after.self_deaf:
                    await channel.send(ts+'***'+user+'*** fully muted by himself')
                else:
                    await channel.send(ts+'***'+user+'*** fully unmuted by himself')
            elif after.self_mute != before.self_mute:
                if after.self_mute:
                    await channel.send(ts+'***'+user+'*** muted by himself')
                else: 
                    await channel.send(ts+'***'+user+'*** unmuted by himself')
            elif after.self_stream != before.self_stream:
                if after.self_stream:
                    await channel.send(ts+'***'+user+'*** started streaming')
                else: 
                    await channel.send(ts+'***'+user+'*** stopped streaming')
            elif after.self_video != before.self_video:
                if after.self_video:
                    await channel.send(ts+'***'+user+'*** shows himself in video call')
                else:
                    await channel.send(ts+'***'+user+'*** hides himself in video call')

@bot.command()
async def setchannel(ctx):
    global channel
    if ctx.channel.name == 'bot-mod':
        try:
            channel = ctx.message.channel_mentions[0]
            await ctx.send('Log channel set to: '+channel.mention)
        except IndexError:
            await ctx.send('Bruh you forget the channel lel')
            pass

@bot.command()
async def setabcd(ctx):
    global abcd
    if ctx.channel.name == 'bot-mod':
        if abcd != None:
            abcd = not abcd
        else:
            abcd = False
        await ctx.send('ABCD set to '+str(abcd))

@bot.event
async def on_message(message):
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
    if bot.user.mentioned_in(message) and not message.author.bot:
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

    await bot.process_commands(message)

    #Check if the send attachment is an image
    if abcd:
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

#Bot reaction to an image -> leaving one reaction
@bot.event
async def on_reaction_add(reaction,user):
    if abcd:
        for react in reaction.message.reactions:
            async for users in react.users (limit=None,after=None):
                if react != reaction and users == user and user != bot.user:
                    await reaction.remove(user)
