import asyncio
import discord
from discord.ext import commands
import re

import pafy
import urllib.request

FFMPEG_OPTIONS = {'before_options': '-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5','options': '-vn'}

class YTPlayer(commands.Cog):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.voice = None

    def cog_unload(self):
        pass

    def cog_check(self, ctx: commands.Context):
        if not ctx.guild:
            raise commands.NoPrivateMessage('This command can\'t be used in DM channels.')
        return True
    
    async def cog_command_error(self, ctx: commands.Context, error: commands.CommandError):
        await ctx.send("Nie mam ochoty działać poprawnie | "+ str(error))

    @commands.command(name='play')
    async def _jutub(self,ctx: commands.Context, *args):     
        """Play audio form youtube. Uses first result of a youtube search."""
        
        channel = ctx.message.author.voice.channel
        if(len(self.bot.voice_clients) == 0 and channel != None):                              
            search = ""
            for x in args:
                search = search + str(x) + "+" 
            html = urllib.request.urlopen("https://www.youtube.com/results?search_query=" + search)
            video_ids = re.findall(r"watch\?v=(\S{11})", html.read().decode())
            await ctx.send("https://www.youtube.com/watch?v=" + video_ids[0])
            song = pafy.new(video_ids[0])
            url = song.getbestaudio().url
            
            self.voice = await channel.connect()                      
            source = discord.FFmpegPCMAudio(url, **FFMPEG_OPTIONS)
            self.voice.play(source)
            while(self.voice.is_playing() or self.voice.is_paused()):
                await asyncio.sleep(1)
            await self.voice.disconnect()                    

    @commands.command(name='pause')
    async def _pause(self,ctx: commands.Context):
        """Pause currently played sound"""
    
        self.voice.pause()

    @commands.command(name='resume')
    async def _resume(self,ctx: commands.Context):
        """Resume audio playback"""  
        
        self.voice.resume()

    @commands.command(name='stop')
    async def _stop(self,ctx: commands.Context):
        """Stop audio playpack"""
        
        self.voice.stop()

def setup(bot):
    """Add component"""
    
    bot.add_cog(YTPlayer(bot))
