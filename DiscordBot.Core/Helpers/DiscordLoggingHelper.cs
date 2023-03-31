using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Helpers
{
    /// <summary>
    /// Helper for logging <see cref="LogMessage"/>
    /// </summary>
    public interface IDiscordLoggingHelper
    {
        /// <summary>
        /// Converts <see cref="LogSeverity"/> to <see cref="LogLevel"/>
        /// </summary>
        /// <param name="logSeverity">Log severity</param>
        /// <returns>Log level matching given log severity</returns>
        LogLevel ConvertLogLevel(LogSeverity logSeverity);

        /// <summary>
        /// Implementation of logging <see cref="LogMessage"/> using <see cref="ILogger{DiscordSocketClient}"/>.
        /// </summary>
        /// <param name="logger">Logger used to log.</param>
        /// <param name="logMessage">Discord's log event</param>
        /// <returns></returns>
        Task LogDiscordLogMessage(ILogger<DiscordSocketClient> logger, LogMessage logMessage);
    }

    /// <inheritdoc cref="IDiscordLoggingHelper"/>
    public class DiscordLoggingHelper : IDiscordLoggingHelper
    {
        public Task LogDiscordLogMessage(ILogger<DiscordSocketClient> logger, LogMessage logMessage)
        {
            using (logger.BeginScope(new Dictionary<string, object>() { { "Source", logMessage.Source } }))
                logger.Log(ConvertLogLevel(logMessage.Severity), logMessage.Exception, logMessage.Message);
            return Task.CompletedTask;
        }

        public LogLevel ConvertLogLevel(LogSeverity logSeverity)
        {
            return logSeverity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Verbose => LogLevel.Debug,
                _ => throw new NotSupportedException($"Cannot translate log severity {logSeverity}. Translation unknown.")
            };
        }
    }
}
