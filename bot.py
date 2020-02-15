import asyncio
import discord
from discord.ext import commands

bot = commands.Bot(command_prefix= '?')

@bot.event
async def on_ready():
    print('Logged in as {0.user}'.format(bot))

@bot.event
async def on_voice_state_update(member, before, after):
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
                        await voiceBot.disconnect()

@bot.command()
async def on(ctx):
    await ctx.send(':nyan:')

@bot.command()
async def cls(ctx):
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        await ctx.channel.purge()

@bot.command()
async def join(ctx, channelName):
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        for channel in ctx.guild.channels:
            if channel.name == channelName:
                await channel.connect()
                break

@bot.command()
async def saytts(ctx, channelName, message):
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        for channel in ctx.guild.channels:
            if channel.name == channelName:
                await channel.send(content = message, tts = True)
                break

@bot.command()
async def say(ctx, channelName, message):
    if ctx.channel.type.name == 'text' and ctx.channel.name == 'bot-mod':
        for channel in ctx.guild.channels:
            if channel.name == channelName:
                await channel.send(content = message)
                break

@bot.command()
async def fprop(ctx, *args):
    file = open(r'function_propositions.txt', 'a')
    file.write(str(ctx.author) + ':\t' + ' '.join(args) + '\n')
    file.close()

@bot.event
async def on_message(message):
    if message.channel.type.name == 'text' and message.channel.type.name != 'bot-mod':
        file = open('message logs/{}_history.log'.format(message.channel.name), 'a')
        file.write(message.created_at.strftime('%d/%m %H:%M:%S\t') + str(message.author) + ':\t' + message.content + '\n')
        file.close()
    await bot.process_commands(message)

bot.run('NTk4MjA2MjE5NzY4OTU0OTQw.XkbsZw.R9tjPAwKThMPMhzdTMpJQ79yyQw')