using Discord;
using Discord.Audio;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Helpers.Models;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DiscordBot.Commands
{
    [Name("yt")]
    [Description("Plays audio from youtube")]
    [Option("url", "Link to YouTube video", CommandOptionType.String, false)]
    [Option("query", "Youtube search phrase", CommandOptionType.String, false)]
    [Option("channel", "voice channel", CommandOptionType.GuildVoiceChannel)]
    public class YoutubeCommand : Command
    {
        private readonly IAudioClientManager _audioClientManager;
        private readonly ILogger<YoutubeCommand> _logger;
        private Dictionary<ulong, Queue<YoutubeVideoData>> _videoQueues;

        public YoutubeCommand(IAudioClientManager audioClientManager, ILogger<YoutubeCommand> logger)
        {
            _audioClientManager = audioClientManager;
            _logger = logger;
            _videoQueues = new();
        }

        public override Task ExecuteAsync(SocketSlashCommand command)
        {
            CheckArguments(command).Wait();
            command.DeferAsync().Wait();
            Task.Run(async () =>
            {
                var targetChannel = command.GetRequiredOptionValue("channel") as SocketChannel;
                if (targetChannel is SocketVoiceChannel voiceChannel)
                {
                    var videoData = command.GetOptionValue("url") is not null ? GetVideoDataFromUri(command.GetOptionValue("url") as string) : GetVideoDataFromQuery(command.GetOptionValue("query") as string);

                    if (!_videoQueues.ContainsKey(command.GuildId.Value))
                        _videoQueues.Add(command.GuildId.Value, new());
                    var queue = _videoQueues[command.GuildId.Value];
                    queue.Enqueue(videoData);
                    if (queue.Count > 1)
                    {
                        await command.ModifyOriginalResponseAsync(m => { m.Embed = BuildEmbed(videoData, $"Added to queue on {queue.Count - 1} position"); m.Content = null; });
                    }
                    else
                    {
                        await command.ModifyOriginalResponseAsync(m => { m.Embed = BuildEmbed(videoData, "Now playing"); m.Content = null; });
                        var skipMsg = true;
                        _logger.LogDebug("Connecting to channel {vc}", voiceChannel.Name);
                        var audioClient = await _audioClientManager.JoinChannelAsync(voiceChannel);
                        try
                        {
                            while (queue.Count > 0)
                            {
                                var playingVideoData = queue.Peek();
                                if (!skipMsg)
                                    await command.Channel.SendMessageAsync(embed: BuildEmbed(playingVideoData, "Now playing"));
                                else
                                    skipMsg = false;
                                await PlayAudio(playingVideoData, audioClient);
                                queue.Dequeue();
                            }

                        }
                        finally
                        {
                            _logger.LogDebug("Leaving channel");
                            await _audioClientManager.LeaveChannelAsync(voiceChannel);
                            _logger.LogDebug("Clearing video queue for guild");
                            queue.Clear();
                        }
                    }
                }
            });
            return Task.CompletedTask;
        }

        private async Task PlayAudio(YoutubeVideoData videoData, IAudioClient audioClient)
        {
            _logger.LogDebug("Getting audio player for channel");
            var player = _audioClientManager.GetAudioPlayer(audioClient);
            _logger.LogDebug("Starting yt/ffmpeg process");
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
    
        private async Task CheckArguments(SocketSlashCommand command)
        {
            var uri = command.GetOptionValue("url");
            var query = command.GetOptionValue("query");
            if (uri is null && query is null)
            {
                _logger.LogDebug("Called command with neither uri nor query parameters.");
                throw new ArgumentException("Called command with neither uri nor query parameters.");
            }
            if (uri is not null && query is not null)
            {
                _logger.LogDebug("Called command with both uri and query parameters.");
                throw new ArgumentException("Called command with both uri and query parameters.");
            }
        }

        private YoutubeVideoData GetVideoDataFromUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                _logger.LogWarning("{Uri} was not a proper uri", uri);
                throw new ArgumentException($"{uri} was not a proprt uri", nameof(uri));
            }
            _logger.LogDebug("Getting yt metadata for {Uri}", uri);
            return GetVideoData(uri);
        }

        private YoutubeVideoData GetVideoDataFromQuery(string query)
        {
            _logger.LogDebug($"Getting yt metadata for search query: \"{query}\"");
            return GetVideoData($"ytsearch1:\"{query}\"");
        }

        private YoutubeVideoData GetVideoData(string endQuery)
        {   
            using (var youtubeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"--print-json --skip-download -f 251/250/249 {endQuery}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            }))
            return JsonConvert.DeserializeObject<YoutubeVideoData>(youtubeProcess.StandardOutput.ReadToEnd())!;
        }

        private Embed BuildEmbed(YoutubeVideoData videoData, string title)
        {
            var builder = new EmbedBuilder()
            {
                Title = title,
                ThumbnailUrl = videoData.ThumbnailUrl,
                Url = videoData.YoutubeUrl,
                Description = $"Title: *** {videoData.Title} *** \nTime: {ConvertVideoDuratiuon(videoData.Duration)}"
            };
            return builder.Build();
        }

        private string ConvertVideoDuratiuon(string videoDuratiuon)
        {
            if (videoDuratiuon.Contains(':'))
                return videoDuratiuon;
            else if (videoDuratiuon.Length == 2)
                return $"0:{videoDuratiuon}";
            else if (videoDuratiuon.Length == 1)
                return $"0:0{videoDuratiuon}";
            else
                _logger.LogWarning("Cannot handle video duration format. Input string: {VideoDuration}", videoDuratiuon);
            return videoDuratiuon;
        }
    }
}
