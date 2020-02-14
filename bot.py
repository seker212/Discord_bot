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

bot.run('NTk4MjA2MjE5NzY4OTU0OTQw.XkbsZw.R9tjPAwKThMPMhzdTMpJQ79yyQw')
