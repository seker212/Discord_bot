using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Helpers.Models;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Helpers
{
    public interface IVoiceMessagesSenderHelper
    {
        void SendNowPlayingEmbed(YoutubeVideoData videoData, string message, SocketSlashCommand command, bool isResponse);
        void SendNowPlaying(string message, SocketSlashCommand command, bool isResponse);
    }

    public class VoiceMessagesSenderHelper : IVoiceMessagesSenderHelper
    {
        private readonly ILogger<VoiceMessagesSenderHelper> _logger;

        public VoiceMessagesSenderHelper(ILogger<VoiceMessagesSenderHelper> logger)
        {
            _logger = logger;
        }

        public void SendNowPlayingEmbed(YoutubeVideoData videoData, string message, SocketSlashCommand command, bool isResponse)
        {
            if (isResponse)
                command.ModifyOriginalResponseAsync(m => { m.Embed = BuildEmbed(videoData, message); m.Content = null; });
            else
                command.Channel.SendMessageAsync(embed: BuildEmbed(videoData, message));
        }

        public void SendNowPlaying(string message, SocketSlashCommand command, bool isResponse)
        {
            if (isResponse)
                command.ModifyOriginalResponseAsync(m => { m.Content = message; });
            else
                command.Channel.SendMessageAsync(message);
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
