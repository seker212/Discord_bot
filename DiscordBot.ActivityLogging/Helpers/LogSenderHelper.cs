using Discord.WebSocket;
using Discord;
using DiscordBot.ActivityLogging.Enums;
using DiscordBot.ActivityLogging.Helpers.Models;
using DiscordBot.Core.Helpers;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.ActivityLogging.Helpers
{
    /// <summary>
    /// Responsible for providing common log method
    /// </summary>
    public interface ILogSenderHelper
    {
        /// <summary>
        /// Method that send activity logs on disord channel based on provided content
        /// </summary>
        /// <param name="content">Activity object that will be logged</param>
        /// <returns>Task for discord <see cref="IMessageChannel.SendMessageAsync">SendMessageAsync()</see></returns>
        Task SendLogsToChannel(LogActivityContent content);
    }

    /// <inheritdoc cref="ILogSenderHelper"/>
    public class LogSenderHelper : ILogSenderHelper
    {
        private readonly IConfigProvider _configProvider;
        private readonly ITimezoneHelper _timeZoneHelper;
        private readonly IDiscordClient _client;
        private readonly ILogger<LogSenderHelper> _logger;

        public LogSenderHelper(IConfigProvider configProvider, ITimezoneHelper timeZoneHelper, IDiscordClient client, ILogger<LogSenderHelper> logger)
        {
            _configProvider = configProvider;
            _timeZoneHelper = timeZoneHelper;
            _client = client;
            _logger = logger;
        }

        public async Task SendLogsToChannel(LogActivityContent content)
        {
            var loggingChannel = await GetLoggingTextChannel(content.GuildId);
            
            switch (content.LogActivityType)
            {
                case LogActivityType.VoiceChannelActivity:
                    await SendVoiceChannelActivity(loggingChannel, content.Mention, content.VoiceAction);
                    break;
                case LogActivityType.TextChannelActivityMessageRemoved:
                    await SendTextChannelActivity(loggingChannel, content.Mention, content.TextAction, "Message removed");
                    break;
            }
        }

        /// <summary>
        /// Method for sending formatted information about text activity
        /// </summary>
        /// <param name="textChannel">Discord text channel that message will be send</param>
        /// <param name="mention">User mention associated with message</param>
        /// <param name="action">What text activity was done in text form</param>
        /// <param name="title">Title of log message</param>
        /// <returns>Task for discord <see cref="IMessageChannel.SendMessageAsync">SendMessageAsync()</see></returns>
        private async Task SendTextChannelActivity(SocketTextChannel textChannel, string mention, string action, string title)
        {
            var time = GetTime(textChannel.Guild.Id);
            var embed = BuildEmbed(title, $"[ {time} ] \r\n" + action);
            await textChannel.SendMessageAsync(embed: embed);
        }

        /// <summary>
        /// Method for sending formatted information about voice activity.
        /// </summary>
        /// <param name="textChannel">Discord text channel that message will be send</param>
        /// <param name="mention">User mention associated with message</param>
        /// <param name="action">What text activity was done in text form</param>
        /// <returns>Task for discord <see cref="IMessageChannel.SendMessageAsync">SendMessageAsync()</see></returns>
        private async Task SendVoiceChannelActivity(SocketTextChannel textChannel, string mention, string action)
        {
            var embed = BuildEmbed("Voice status change", $"[ {GetTime(textChannel.Guild.Id)} ]" + mention + " " + action);

            await textChannel.SendMessageAsync(embed: embed);
        }

        /// <summary>
        /// Method for obtaning time for specific guildId.
        /// </summary>
        /// <param name="guildId">Discord unique guild id</param>
        /// <returns>Formatted current time in guild timezone</returns>
        private string GetTime(ulong guildId)
        {
            var timezoneString = _configProvider.GetParameter(guildId, "TimeZoneValue");
            var timezone = _timeZoneHelper.ConvertTimeZoneFromString(timezoneString);
            var time = _timeZoneHelper.GetCurrentTimeZoneTime(timezone);
            return time;
        }

        /// <summary>
        /// Obtains guilds log channel based on database value.
        /// </summary>
        /// <param name="guildId">Discord unique guild id</param>
        /// <returns>Text channel as Task</returns>
        private async Task<SocketTextChannel?> GetLoggingTextChannel(ulong guildId)
        {
            var textChannelId = _configProvider.GetParameter(guildId, "LoggingChannelId");
            return textChannelId is null ? null : await _client.GetChannelAsync(Convert.ToUInt64(textChannelId)) as SocketTextChannel;
        }

        /// <summary>
        /// Method for building discord embed message with common formatting for logs.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns>Discord built embed</returns>
        private Embed BuildEmbed(string title, string description)
        {
            var embed = new EmbedBuilder()
            {
                Title = title,
                Description = description,
            };

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.DarkGrey);

            return embed.Build();
        }
    }
}
