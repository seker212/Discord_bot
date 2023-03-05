using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;

namespace DiscordBot.Commands
{
    [Name("sound")]
    [Description("Plays sound")]
    public class SoundCommand : Command
    {
        private readonly IAudioClientManager _audioClientManager;
        private readonly ILogger<SoundCommand> _logger;

        public SoundCommand(IAudioClientManager audioClientManager, ILogger<SoundCommand> logger)
        {
            _audioClientManager = audioClientManager;
            _logger = logger;
        }

        public override SlashCommandBuilder CustomBuildAction(SlashCommandBuilder slashCommandBuilder) //TODO: Add attribute for basic options
        {
            var soundNameOptionBuilder = new SlashCommandOptionBuilder();
            soundNameOptionBuilder
                .WithName("soundname")
                .WithDescription("sound name")
                .WithType(ApplicationCommandOptionType.String);

            var vcOptionBuilder = new SlashCommandOptionBuilder();
            vcOptionBuilder
                .WithName("channel")
                .WithDescription("voice channel")
                .WithType(ApplicationCommandOptionType.Channel);

            return slashCommandBuilder.AddOptions(vcOptionBuilder, soundNameOptionBuilder);
        }

        public override Task ExecuteAsync(SocketSlashCommand command)
        {
            Task.Run(async () =>
            {
                try
                {
                    var soundName = command.Data.Options.Single(x => x.Name == "soundname").Value as string;
                    var audioFile = new FileInfo(Path.Combine(@"..\..\..\..\..\audio\", $"{soundName}.mp3"));
                    if (!audioFile.Exists)
                    {
                        var message = $"Sound {soundName} not found";
                        if (command.HasResponded)
                            await command.ModifyOriginalResponseAsync(m => m.Content = message);
                        else
                            await command.RespondAsync(message);
                        throw new FileNotFoundException(audioFile.FullName);
                    }
                    else
                    {
                        var targetChannel = command.Data.Options.Single(x => x.Name == "channel").Value as SocketChannel;
                        if (targetChannel is SocketVoiceChannel voiceChannel)
                        {
                            var audioClient = await _audioClientManager.JoinChannelAsync(voiceChannel);
                            try
                            {
                                var player = _audioClientManager.GetAudioPlayer(audioClient);
                                using (var ffmpegProcess = Process.Start(new ProcessStartInfo
                                {
                                    FileName = "ffmpeg.exe",
                                    Arguments = $"-hide_banner -loglevel panic -i \"{audioFile.FullName}\" -ac 2 -f s16le -ar 48000 pipe:1",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true
                                }))
                                    await player.PlayAsync(ffmpegProcess.StandardOutput.BaseStream);
                            }
                            finally
                            {
                                await _audioClientManager.LeaveChannelAsync(voiceChannel);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sound command threw an exception");
                }
            });
            return command.RespondAsync("placeholder");
        }
    }
}
