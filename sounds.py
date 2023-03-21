import asyncio
import discord
import os, time
from discord.ext import commands

from gtts import gTTS

import logger as log
logger = log.getLogger(__name__)

class Sounds(commands.Cog):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.voice = None
        self.alphabet = ['A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','V','W','Y','Z']

        logger.info("Sound constructor finished")

        if os.path.isdir("/tmp"):
            logger.info("tmp folder does not exists, creating...")
            os.mkdir('tmp')

    def cog_unload(self):
        pass

    def cog_check(self, ctx: commands.Context):
        if not ctx.guild:
            raise commands.NoPrivateMessage('This command can\'t be used in DM channels.')

        return True
    
    async def cog_command_error(self, ctx: commands.Context, error: commands.CommandError):
        await ctx.send(ctx.author.mention+" Sound Error look at this: "+ str(error))

    async def _removeFile(self, path):
        """Removes file from given path"""

        logger.info("Removing file: "+path)
        os.remove(path)

    async def _playInChannel(self, channel, path, removeFile=False):
        """Plays from file from given path to given channel"""

        logger.info("Playing sound in channel: ChannelId: "+str(channel.id)+" ChannelName: "+str(channel.name)+" Path: "+str(path))       

        self.voice = await channel.connect()
        await asyncio.sleep(0.5)      
        self.voice.play(discord.FFmpegPCMAudio(path), after = self.voice.stop())
        while(self.voice.is_playing()):
            await asyncio.sleep(1)
        await self.voice.disconnect()

        if removeFile:
            await self._removeFile(path)

    async def _play(self, ctx: commands.Context, soundType=None, targetFolder=None, targetChannel=None, removeFile=False):
        """Abstract sound command to play in given context"""

        logger.info("Playing sound in context: GuildId: "+str(ctx.guild.id)+" ChannelId: "+str(ctx.channel.id)+" AuthorId: "+str(ctx.author.id))

        path = targetFolder+soundType+'.mp3'
        if not os.path.isfile(path):
            await ctx.send("File search failed. File: "+soundType)
            return

        if len(self.bot.voice_clients) == 0:
            member = ctx.message.author
            for channel in member.guild.channels:
                if channel.type == discord.ChannelType.voice:
                    if channel.name == targetChannel:
                        await ctx.send("Playing now: "+soundType)
                        await self._playInChannel(channel, path, removeFile=removeFile)
                        return              
                    if targetChannel == None:
                        for m in channel.voice_states:
                            if m == member.id:
                                await ctx.send("Playing now: "+soundType)
                                await self._playInChannel(channel, path, removeFile=removeFile)
                                return
            await ctx.send("No channel found to play: "+soundType)
        else:
            await ctx.send("Impossible to play: "+soundType)

    @commands.command(name='sound')
    async def _sound(self,ctx: commands.Context,soundType):
        """Arrives to you and plays your favourite sound"""

        await self._play(ctx, soundType, "./audio/")

    @commands.command(name='soundchannel')
    @commands.has_permissions(manage_guild=True)
    async def _soundchannel(self, ctx: commands.Context,soundType, *args):
        """Arrives to you and plays your favourite sound in given channel"""

        channel = None
        if len(args) > 0:
            channel = ' '.join(args)

        logger.info("Playing sound: "+soundType+ " in channel: "+str(channel))

        await self._play(ctx, soundType, "./audio/", targetChannel=channel)

    @commands.command(name="soundtts")
    async def _soundtts(self, ctx: commands.Context, *args):
        """Arrives to you and plays given message"""

        logger.info("Joining channel and speaking message: "+str(args))

        tmpName = str(int(time.time()*1000.0))
        text = ''

        if len(args) == 0:
            await ctx.send("No message given")
            return
        
        text = ' '.join(args)
        tts = gTTS(text=text, lang='pl')
        tts.save('tmp/'+tmpName+'.mp3')

        await self._play(ctx, soundType=tmpName, targetFolder="./tmp/", removeFile=True)

    @commands.command(name='soundlist')
    async def _soundList(self, ctx: commands.Context):
        """Prints the list of all avaible sounds"""

        logger.info("List of sounds")

        embed = discord.Embed(title="Sounds list",description="All of the sounds",color=0x076500)

        sounds = os.listdir('./audio')

        for char in self.alphabet:
            s = ''
            for sound in sounds:
                if sound[:1].upper() == char:
                    s += sound[:-4] + '\n'
            if s != '':
                embed.add_field(name=char, value=s)

        await ctx.send(embed=embed)

    @commands.command(name='soundupload')
    @commands.has_permissions(manage_guild=True)
    async def _uploadSound(self, ctx: commands.Context):
        """Attach a sound to be uploaded with this command"""
 
        logger.info("Uploading sound")

        if len(ctx.message.attachments) == 1:
            file = ctx.message.attachments[0].filename
            path = "audio/{}".format(file)
            if file.endswith(".mp3") and not os.path.isfile(path):
                await ctx.message.attachments[0].save(fp=path)
                logger.info("Uploaded sound: {}".format(file))

    @commands.command(name='soundrm')
    @commands.has_permissions(manage_guild=True)
    async def _removeSound(self, ctx: commands.Context, sound):
        """Removes a given sound"""
        
        logger.info("Removing sound: "+sound)

        try:
            path = './audio/'+sound+'.mp3' 
            os.remove(path) 
            await ctx.send('Removed: '+sound)
        except Exception:
            raise

async def setup(bot):
    """Add component"""

    logger.info("Adding cog " + __name__)
    await bot.add_cog(Sounds(bot))
