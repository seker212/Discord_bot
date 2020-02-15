import asyncio
import discord
from discord.ext import commands

bot = commands.Bot(command_prefix= '?')

@bot.event
async def on_ready():
    print('Logged in as {0.user}'.format(bot))

@bot.command()
async def on(ctx):
    await ctx.send(':nyan:')

@bot.command()
async def cls(ctx):
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        await ctx.channel.purge()

@bot.command()
async def join(ctx, channelName):
    for channel in ctx.guild.channels:
        if channel.name == channelName:
            await channel.connect()
            break

@bot.event
async def on_voice_state_update(member, before, after):
    if not member.bot:
        if after.channel != before.channel: #only channel change
            if after.channel != None:       #someone on a channel
                if after.channel.name == 'Klocki' and len(after.channel.members) == 1:
                    if len(bot.voice_clients) == 0:
                        voiceBot = await after.channel.connect()
                        file = open('Get_out_minecraft.raw', 'rb')
                        voiceBot.play(discord.PCMAudio(file), after = voiceBot.stop())
                        while voiceBot.is_playing():
                            await asyncio.sleep(1)
                        await voiceBot.disconnect()



bot.run('NTk4MjA2MjE5NzY4OTU0OTQw.XkbsZw.R9tjPAwKThMPMhzdTMpJQ79yyQw')