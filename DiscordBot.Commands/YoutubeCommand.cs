using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Helpers.Models;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DiscordBot.Commands
{
    [Name("yt")]
    [Description("yt play")]
    [Option("url", "url", ApplicationCommandOptionType.String)]
    [Option("channel", "voice channel", ApplicationCommandOptionType.Channel)]
    public class YoutubeCommand : Command
    {
        private readonly IAudioClientManager _audioClientManager;
        private readonly ILogger<YoutubeCommand> _logger;

        public YoutubeCommand(IAudioClientManager audioClientManager, ILogger<YoutubeCommand> logger)
        {
            _audioClientManager = audioClientManager;
            _logger = logger;
        }

        public override Task ExecuteAsync(SocketSlashCommand command)
        {
            command.DeferAsync().Wait();
            Task.Run(async () =>
            {
                var targetChannel = command.Data.Options.Single(x => x.Name == "channel").Value as SocketChannel;
                if (targetChannel is SocketVoiceChannel voiceChannel)
                {
                    var uri = command.Data.Options.Single(x => x.Name == "url").Value as string;
                    var videoData = GetVideoDataFromUri(uri);
                    _logger.LogDebug("Connecting to channel {vc}", voiceChannel.Name);
                    var audioClient = await _audioClientManager.JoinChannelAsync(voiceChannel);
                    try
                    {
                        _logger.LogDebug("Getting audio player for channel {voiceChannelName}", voiceChannel.Name);
                        var player = _audioClientManager.GetAudioPlayer(audioClient);
                        _logger.LogDebug("Starting yt process");
                        using (var ffmpegProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = "ffmpeg",
                            Arguments = $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -hide_banner -loglevel panic -i \"{videoData.DownloadUrl}\" -ac 2 -f s16le -ar 48000 pipe:1",
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
                        await command.ModifyOriginalResponseAsync(m => m.Content = $"Finished playing \"{videoData.Title}\"");
                    }
                }
            });
            return Task.CompletedTask;
        }
    
        private YoutubeVideoData GetVideoDataFromUri(string uri)
        {   
            string args = $"--print-json --skip-download -f 251/250/249 {uri}";
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                args = $"--print-json --skip-download -f 251/250/249 ytsearch1:\"{uri}\"";
                //_logger.LogWarning("{Uri} was not proper uri", uri);
                //throw new ArgumentException($"{uri} was not a proprt uri", nameof(uri));
            }
            _logger.LogDebug("Getting yt metadata for {Uri}", uri);
            using (var youtubeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true
            }))
            return JsonConvert.DeserializeObject<YoutubeVideoData>(youtubeProcess.StandardOutput.ReadToEnd())!;
        }
    }
}
