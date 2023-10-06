using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Helpers.Models;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Helpers
{
    /// <summary>
    /// Helper for sending messages about voice updates
    /// </summary>
    public interface IVoiceMessagesSenderHelper
    {
        /// <summary>
        /// Method for sending messages in embed, used primarly for youtube.
        /// </summary>
        /// <param name="videoData">Youtube video data that will be used in embed</param>
        /// <param name="message">Core message of embed (Title)</param>
        /// <param name="command">Command that was used</param>
        /// <param name="isResponse">For checking if send message should be a response to command or new message</param>
        void SendNowPlayingEmbed(YoutubeVideoData videoData, string message, SocketSlashCommand command, bool isResponse);

        /// <summary>
        /// Method for sending basic messages, used primarly for sound commands.
        /// </summary>
        /// <param name="message">Text that will be send</param>
        /// <param name="command">Command that was used</param>
        /// <param name="isResponse">For checking if send message should be a response to command or new message</param>
        void SendNowPlaying(string message, SocketSlashCommand command, bool isResponse);
    }

    /// <inheritdoc cref="IVoiceMessagesSenderHelper"/>
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

        /// <summary>
        /// Method for creating embed with datails about youtube video.
        /// </summary>
        /// <param name="videoData">Youtube object containing information about video</param>
        /// <param name="title">Title of embed</param>
        /// <returns>Built embed</returns>
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

        /// <summary>
        /// Method for converting time from youtube video. 
        /// If videos is shorter than minute it required additional 0.
        /// </summary>
        /// <param name="videoDuratiuon">Time in string format e.g 1:23, 23, 3</param>
        /// <returns>
        /// Converted time or videoDuration if conversion failed.
        /// e.g 1:23. 0:23, 0:03
        /// </returns>
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
