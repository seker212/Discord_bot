using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Helpers
{
    /// <summary>
    /// Helper for resolving voice channels.
    /// </summary>
    public interface IVoiceChannelResolver
    {
        /// <summary>
        /// Method for obtaining voice channel based on command parameter and usert status.
        /// </summary>
        /// <param name="command">Command that was executed</param>
        /// <param name="optionName">Name of option where voice channel was passed</param>
        /// <returns>
        /// If parameter was correct voice channel it will be returned.
        /// If user is present in voice channel, this channel will be returned.
        /// Null otherwise, with direct response to command.
        /// </returns>
        IVoiceChannel? GetVoiceChannel(SocketSlashCommand command, string optionName);
    }

    /// <inheritdoc cref="IVoiceChannelResolver"/>
    public class VoiceChannelResolver : IVoiceChannelResolver
    {
        private readonly ILogger<VoiceChannelResolver> _logger;

        public VoiceChannelResolver(ILogger<VoiceChannelResolver> logger)
        {
            _logger = logger;
        }

        public IVoiceChannel? GetVoiceChannel(SocketSlashCommand command, string optionName)
        {
            var channel = command.GetOptionValue<IVoiceChannel>(optionName);
            if (channel != null) 
            {
                return channel;
            }

            var guildId = command.GuildId;

            if(guildId == null)
            {
                SendErrorResponseAsync(command, "Command used in wrong context, user withoud guildId");
                return null;
            }

            var user = command.User as IGuildUser;
            var voiceChannel = user.VoiceChannel;

            if(voiceChannel == null)
            {
                SendErrorResponseAsync(command, "User currently not in any voice channel, use channel parameter or join channel");
                return null;
            }

            return voiceChannel;
        }

        /// <summary>
        /// Sends response to command if error occured while resolving channel and logs it.
        /// </summary>
        /// <param name="command">Command to respond to</param>
        /// <param name="text">Error message</param>
        private async void SendErrorResponseAsync(SocketSlashCommand command, string text)
        {
            _logger.LogError(text);
            await command.ModifyOriginalResponseAsync(m => { m.Content = text; });
        }
    }
}
