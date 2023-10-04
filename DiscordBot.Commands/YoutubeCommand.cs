using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Helpers;
using DiscordBot.Commands.Helpers.Models;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("yt")]
    [Description("Plays audio from youtube")]
    [Option("text", "Link to YouTube video or query (text) to search for video", CommandOptionType.String, true)]
    [Option("channel", "voice channel", CommandOptionType.GuildVoiceChannel, false)]
    public class YoutubeCommand : Command
    {
        private readonly IAudioQueueManager _audioQueueManager;
        private readonly IAudioStreamHelper _audioStreamHelper;
        private readonly IYoutubeSearchHelper _youtubeSearchHelper;
        private readonly IVoiceChannelResolver _voiceChannelResolver;
        private readonly ILogger<YoutubeCommand> _logger;

        public YoutubeCommand(IAudioQueueManager audioQueueManager, ILogger<YoutubeCommand> logger, IAudioStreamHelper audioStreamHelper, IYoutubeSearchHelper youtubeSearchHelper, IVoiceChannelResolver voiceChannelResolver)
        {
            _audioQueueManager = audioQueueManager;
            _audioStreamHelper = audioStreamHelper;
            _logger = logger;
            _youtubeSearchHelper = youtubeSearchHelper;
            _voiceChannelResolver = voiceChannelResolver;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();

            var targetChannel = _voiceChannelResolver.GetVoiceChannel(command);

            if(targetChannel == null)
            {
                return;
            }

            var urlOrQuery = command.GetOptionValue<string>("text")!;
            var videoData = _youtubeSearchHelper.GetYoutubeVideoData(urlOrQuery);
            var queueCount = _audioQueueManager.GetQueueCount(command.GuildId.Value);

            var queueEntry = new AudioQueueEntry(
                targetChannel,
                new Lazy<AudioStreamElements>(() => _audioStreamHelper.CreateAudioStream(videoData)),
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
