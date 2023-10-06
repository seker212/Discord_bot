using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Helpers;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.VoiceControlCommands
{
    [Name("yt")]
    [Description("Plays audio from youtube")]
    [Option("video", "Link to YouTube video or query (text) to search for video", CommandOptionType.String, true)]
    [Option("channel", "voice channel", CommandOptionType.GuildVoiceChannel, false)]
    public class YoutubeCommand : Command
    {
        private readonly IAudioQueueManager _audioQueueManager;
        private readonly IAudioStreamHelper _audioStreamHelper;
        private readonly IYoutubeSearchHelper _youtubeSearchHelper;
        private readonly IVoiceChannelResolver _voiceChannelResolver;
        private readonly IVoiceMessagesSenderHelper _voiceMessagesSender;
        private readonly ILogger<YoutubeCommand> _logger;

        public YoutubeCommand(IAudioQueueManager audioQueueManager, ILogger<YoutubeCommand> logger, IAudioStreamHelper audioStreamHelper, IYoutubeSearchHelper youtubeSearchHelper, IVoiceChannelResolver voiceChannelResolver, IVoiceMessagesSenderHelper voiceMessagesSender)
        {
            _audioQueueManager = audioQueueManager;
            _audioStreamHelper = audioStreamHelper;
            _logger = logger;
            _youtubeSearchHelper = youtubeSearchHelper;
            _voiceChannelResolver = voiceChannelResolver;
            _voiceMessagesSender = voiceMessagesSender;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();

            var targetChannel = _voiceChannelResolver.GetVoiceChannel(command, "channel");

            if (targetChannel == null)
            {
                return;
            }

            var urlOrQuery = command.GetOptionValue<string>("video")!;
            var videoData = _youtubeSearchHelper.GetYoutubeVideoData(urlOrQuery);
            var queueCount = _audioQueueManager.GetQueueCount(command.GuildId.Value);

            var queueEntry = new AudioQueueEntry(
                targetChannel,
                new Lazy<AudioStreamElements>(() => _audioStreamHelper.CreateAudioStream(videoData)),
                () => _voiceMessagesSender.SendNowPlayingEmbed(videoData, "Now playing", command, queueCount == 0),
                () => _logger.LogDebug("Finished playing"),
                new Dictionary<string, object?>() { { "CommandCallID", command.Id } },
                videoData.Title
                );

            var queuePosition = _audioQueueManager.Add(command.GuildId.Value, queueEntry);
            if (queueCount > 0)
                _voiceMessagesSender.SendNowPlayingEmbed(videoData, $"Added to queue on {queuePosition - 1} position", command, true);
        }
    }
}
