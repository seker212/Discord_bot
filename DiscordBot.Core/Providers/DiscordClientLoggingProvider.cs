using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provides function for logging Discord client's events
    /// </summary>
    public interface IDiscordClientLoggingProvider
    {
        Task LogDiscordClientEvent(LogMessage logMessage);
    }

    /// <inheritdoc cref="IDiscordClientLoggingProvider"/>
    public class DiscordClientLoggingProvider : IDiscordClientLoggingProvider
    {
        private readonly ILogger<DiscordSocketClient> _clientLogger;
        private readonly IDiscordLoggingHelper _discordLoggingHelper;

        public DiscordClientLoggingProvider(ILogger<DiscordSocketClient> clientLogger, IDiscordLoggingHelper discordLoggingHelper)
        {
            _clientLogger = clientLogger;
            _discordLoggingHelper = discordLoggingHelper;
        }

        public Task LogDiscordClientEvent(LogMessage logMessage)
            => _discordLoggingHelper.LogDiscordLogMessage(_clientLogger, logMessage);
    }
}
