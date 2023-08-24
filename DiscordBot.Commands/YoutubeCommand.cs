using Discord;
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
        private readonly IAudioQueueManager _audioQueueManager;
        private readonly ILogger<YoutubeCommand> _logger;

        public YoutubeCommand(IAudioQueueManager audioQueueManager, ILogger<YoutubeCommand> logger)
        {
            _audioQueueManager = audioQueueManager;
            _logger = logger;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            CheckArguments(command);
            await command.DeferAsync();

            var targetChannel = command.GetRequiredOptionValue<IVoiceChannel>("channel");
            var videoData = command.GetOptionValue("url") is not null ? GetVideoDataFromUri(command.GetOptionValue<string>("url")!) : GetVideoDataFromQuery(command.GetOptionValue<string>("query")!);
            var queueCount = _audioQueueManager.GetQueueCount(command.GuildId.Value);

            var queueEntry = new AudioQueueEntry(
                targetChannel,
                new Lazy<AudioStreamElements>(() => CreateAudioStream(videoData)),
                () => SendNowPlaying(videoData, command, queueCount == 0),
                () => _logger.LogDebug("Finished playing"),
                new Dictionary<string, object?>() { { "CommandCallID", command.Id } }
                );
            var queuePosition = _audioQueueManager.Add(command.GuildId.Value, queueEntry); 
            if (queueCount > 0)
                await command.ModifyOriginalResponseAsync(m => { m.Embed = BuildEmbed(videoData, $"Added to queue on {queuePosition - 1} position"); m.Content = null; });
        }

        private void SendNowPlaying(YoutubeVideoData videoData, SocketSlashCommand command, bool isResponse)
        {
            if (isResponse)
                command.ModifyOriginalResponseAsync(m => { m.Embed = BuildEmbed(videoData, "Now playing"); m.Content = null; });
            else
                command.Channel.SendMessageAsync(embed: BuildEmbed(videoData, "Now playing"));
        }

        private AudioStreamElements CreateAudioStream(YoutubeVideoData videoData)
        {
            var ffmpegProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -hide_banner -loglevel panic -i \"{videoData.DownloadUrl}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            return new AudioStreamElements(ffmpegProcess.StandardOutput.BaseStream, new[] { ffmpegProcess });
        }
    
        private void CheckArguments(SocketSlashCommand command)
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
