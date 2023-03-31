using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DiscordBot.Commands
{
    [Name("sound")]
    [Description("Plays sound")]
    [Option("soundname", "sound name", ApplicationCommandOptionType.String)]
    [Option("channel", "voice channel", ApplicationCommandOptionType.Channel)]
    public class SoundCommand : Command
    {
        private const string AUDIO_DIRECTORY_PATH = @"/app/audio";
        private readonly IAudioClientManager _audioClientManager;
        private readonly ILogger<SoundCommand> _logger;

        public SoundCommand(IAudioClientManager audioClientManager, ILogger<SoundCommand> logger)
        {
            _audioClientManager = audioClientManager;
            _logger = logger;
        }

        public override Task ExecuteAsync(SocketSlashCommand command)
        {
            command.DeferAsync().Wait();
            Task.Run(async () =>
            {
                try
                {
                    var soundName = command.Data.Options.Single(x => x.Name == "soundname").Value as string;
                    var audioFile = new FileInfo(Path.Combine(AUDIO_DIRECTORY_PATH, $"{soundName}.mp3"));
                    _logger.LogDebug("Searching audio file in {dir} directory", audioFile.Directory?.FullName);
                    if (!audioFile.Exists)
                    {
                        var message = $"Sound {soundName} not found";
                        if (command.HasResponded)
                            await command.ModifyOriginalResponseAsync(m => m.Content = message);
                        else
                            await command.RespondAsync(message);
                        throw new FileNotFoundException(message, audioFile.FullName);
                    }
                    else
                    {
                        var targetChannel = command.Data.Options.Single(x => x.Name == "channel").Value as SocketChannel;
                        if (targetChannel is SocketVoiceChannel voiceChannel)
                        {
                            _logger.LogDebug("Connecting to channel {vc}", voiceChannel.Name);
                            var audioClient = await _audioClientManager.JoinChannelAsync(voiceChannel);
                            try
                            {
                                _logger.LogDebug("Getting audio player for channel {voiceChannelName}", voiceChannel.Name);
                                var player = _audioClientManager.GetAudioPlayer(audioClient);
                                _logger.LogDebug("Starting ffmpeg process");
                                using (var ffmpegProcess = Process.Start(new ProcessStartInfo
                                {
                                    FileName = "ffmpeg",
                                    Arguments = $"-hide_banner -loglevel panic -i \"{audioFile.FullName}\" -ac 2 -f s16le -ar 48000 pipe:1",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true
                                }))
                                {
                                    _logger.LogDebug("Playing stream");
                                    await player.PlayAsync(ffmpegProcess.StandardOutput.BaseStream);
                                    await Task.Delay(10); //Deleay channel leaving after sound to not instantly hear the leaving sound
                                }
                            }
                            finally
                            {
                                _logger.LogDebug("Leaving channel");
                                await _audioClientManager.LeaveChannelAsync(voiceChannel);
                                await command.ModifyOriginalResponseAsync(m => m.Content = $"Finished playing {soundName}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sound command threw an exception");
                }
            });
            return Task.CompletedTask;
        }
    }
}
