using Discord.WebSocket;
using Discord;
using DiscordBot.ActivityLogging.Enums;
using DiscordBot.ActivityLogging.Helpers.Models;
using DiscordBot.Core.Helpers;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Discord.Rest;

namespace DiscordBot.ActivityLogging.Helpers
{
    public interface ILogSenderHelper
    {
        Task SendLogsToChannel(LogActivityContent content);
    }

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

        private async Task SendTextChannelActivity(SocketTextChannel textChannel, string mention, string action, string title)
        {
            var time = GetTime(textChannel.Guild.Id);
            var embed = BuildEmbed(title, $"[ {time} ] \r\n" + action);
            await textChannel.SendMessageAsync(embed: embed);
        }

        private async Task SendVoiceChannelActivity(SocketTextChannel textChannel, string mention, string action)
        {
            var embed = BuildEmbed("Voice status change", $"[ {GetTime(textChannel.Guild.Id)} ]" + mention + " " + action);

            await textChannel.SendMessageAsync(embed: embed);
        }

        private string GetTime(ulong guildId)
        {
            var timezoneString = _configProvider.GetParameter(guildId, "TimeZoneValue");
            var timezone = _timeZoneHelper.ConvertTimeZoneFromString(timezoneString);
            var time = _timeZoneHelper.GetCurrentTimeZoneTime(timezone);
            return time;
        }

        private async Task<SocketTextChannel?> GetLoggingTextChannel(ulong guildId)
        {
            var textChannelId = _configProvider.GetParameter(guildId, "LoggingChannelId");
            return textChannelId is null ? null : await _client.GetChannelAsync(Convert.ToUInt64(textChannelId)) as SocketTextChannel;
        }

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
