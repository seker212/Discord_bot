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
        /// <summary>
        /// Logs log message from Discord.Net library.
        /// </summary>
        /// <param name="logMessage">Log message.</param>
        /// <returns>Task of logging the message.</returns>
        Task LogDiscordClientEvent(LogMessage logMessage);
    }

    /// <inheritdoc cref="IDiscordClientLoggingProvider"/>
    public class DiscordClientLoggingProvider : IDiscordClientLoggingProvider
    {
        private readonly ILogger<DiscordSocketClient> _clientLogger;
        private readonly IDiscordLoggingHelper _discordLoggingHelper;

        /// <summary>
        /// Creates new <see cref="DiscordClientLoggingProvider"/>.
        /// </summary>
        /// <param name="clientLogger">Logger that the discord log messages will be logged with.</param>
        /// <param name="discordLoggingHelper">Helper with implementation of converting <see cref="LogMessage"/> to log entry.</param>
        public DiscordClientLoggingProvider(ILogger<DiscordSocketClient> clientLogger, IDiscordLoggingHelper discordLoggingHelper)
        {
            _clientLogger = clientLogger;
            _discordLoggingHelper = discordLoggingHelper;
        }

        public Task LogDiscordClientEvent(LogMessage logMessage)
            => _discordLoggingHelper.LogDiscordLogMessage(_clientLogger, logMessage);
    }
}
