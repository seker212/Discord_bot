import asyncio
import discord
import os.path
from discord.ext import commands

class Sounds(commands.Cog):
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
        await ctx.send(ctx.author.mention+" Napierdalamy!!!!!!! | "+ str(error))

    @commands.command(name='sound')
    async def _sound(self,ctx: commands.Context,soundType):
        """Arrives to you and plays your favourite sound"""

        if len(self.bot.voice_clients) == 0 and soundType != None:
            member = ctx.message.author
            await ctx.send("Playing now Darude-Sandstorm")
            for channel in member.guild.channels:
                if channel.type == discord.ChannelType.voice:
                    for m in channel.voice_states:
                        if m == member.id:
                            path = 'audio/'+soundType+'.mp3'
                            if os.path.isfile(path):
                                self.voice = await channel.connect()
                                await asyncio.sleep(0.5)      
                                self.voice.play(discord.FFmpegPCMAudio(path), after = self.voice.stop())
                                while(self.voice.is_playing()):
                                    await asyncio.sleep(1)
                                await self.voice.disconnect()

    @commands.command(name='soundlist')
    async def _soundList(self, ctx: commands.Context):
        """Prints the list of all avaible sounds"""

        embed = discord.Embed(title="Sounds list",description="All of the sounds",color=0x076500)

        sounds = os.listdir('./audio')

        i = 1
        for sound in sounds:
            val = sound[:-4]
            embed.add_field(name=str(i) ,value=val,inline=True)
            i+=1

        await ctx.send(embed=embed)

    @commands.command(name='soundupload')
    @commands.has_permissions(manage_guild=True)
    async def _uploadSound(self, ctx: commands.Context):
        """Attach a sound to be uploaded with this command"""

        if len(ctx.message.attachments) == 1:
            file = ctx.message.attachments[0].filename
            path = "audio/{}".format(file)
            if file.endswith(".mp3") and not os.path.isfile(path):
                await ctx.message.attachments[0].save(fp=path)

    @commands.command(name='soundrm')
    @commands.has_permissions(manage_guild=True)
    async def _removeSound(self, ctx: commands.Context, sound):
        """Removes a given sound"""

        try:
            path = './audio/'+sound+'.mp3' 
            os.remove(path) 
            await ctx.send('Removed: '+sound)
        except Exception:
            raise

def setup(bot):
    """Add component"""

    bot.add_cog(Sounds(bot))
