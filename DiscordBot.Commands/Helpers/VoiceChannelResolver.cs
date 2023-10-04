using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Helpers
{
    public interface IVoiceChannelResolver
    {
        IVoiceChannel? GetVoiceChannel(SocketSlashCommand command);
    }

    public class VoiceChannelResolver : IVoiceChannelResolver
    {
        private readonly ILogger<VoiceChannelResolver> _logger;

        public VoiceChannelResolver(ILogger<VoiceChannelResolver> logger)
        {
            _logger = logger;
        }

        public IVoiceChannel? GetVoiceChannel(SocketSlashCommand command)
        {
            var channel = command.GetOptionValue<IVoiceChannel>("channel");
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

        private async void SendErrorResponseAsync(SocketSlashCommand command, string text)
        {
            _logger.LogError(text);
            await command.ModifyOriginalResponseAsync(m => { m.Content = text; });
        }
    }
}
