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
            video_id = None
            if len(args) == 1:
                web_link_regex_long = r'(https://)?(www\.)?youtube\.com/watch\?.*v=(\S{11}).*'
                web_link_regex_short = r'(https://)?youtu\.be/(\S{11}).*'
                match_long = re.match(web_link_regex_long, args[0])
                match_short = re.match(web_link_regex_short, args[0])
                if match_long != None:
                    video_id = match_long[3]
                elif match_short != None:
                    video_id = match_short[2]

            if video_id == None:
                search = ""
                for x in args:
                    search = search + str(x) + "+" 
                html = urllib.request.urlopen("https://www.youtube.com/results?search_query=" + search)
                video_id = re.findall(r"watch\?v=(\S{11})", html.read().decode())[0]
            
            song = pafy.new(video_id)
            url = song.getbestaudio().url
            
            self.voice = await channel.connect()

            e = discord.Embed(title='Now playing', description=f"Title: *** {song.title} *** \nTime: {song.duration}", url=song.watchv_url)
            e.set_thumbnail(url=song.bigthumb)
            await ctx.send(embed=e)
                         
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
