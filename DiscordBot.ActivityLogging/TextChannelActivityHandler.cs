using Discord.WebSocket;
using Discord;
using DiscordBot.ActivityLogging.Helpers;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using DiscordBot.ActivityLogging.Helpers.Models;
using DiscordBot.ActivityLogging.Enums;

namespace DiscordBot.ActivityLogging
{
    public class TextChannelActivityHandler : ITextChannelActivityHandler
    {
        private const LogActivityType messageRemoved = LogActivityType.TextChannelActivityMessageRemoved;
        
        private readonly ILogSenderHelper _logSenderHelper;
        private readonly ILogger<TextChannelActivityHandler> _logger;

        public TextChannelActivityHandler(ILogSenderHelper logSenderHelper, ILogger<TextChannelActivityHandler> logger)
        {
            _logSenderHelper = logSenderHelper;
            _logger = logger;
        }

        public Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
            => Task.Run(async () =>
            {
                if (!channel.HasValue)
                    return;

                var messageChannel = channel.Value;
                if (messageChannel is not IGuildChannel)
                    return;

                var guildChannel = messageChannel as IGuildChannel;
                var guildId = guildChannel.GuildId;

                if (!message.HasValue)
                    return;

                if(message.Value.Author.IsBot)
                    return;

                var mention = message.Value.Author.Mention;
                var content = message.Value.Content;
                var attachments = message.Value.Attachments;
                var text = $"Author: {mention} \r\nContent: {content} \r\n";
                if(attachments.Count != 0)
                {
                    text += "Attachments urls: \r\n";
                    foreach( var attachment in attachments)
                    {
                        text += attachment.Url;
                        text += "\r\n";
                    }
                }

                var log = new LogActivityContent(messageRemoved, guildId, mention, null, text);
                await _logSenderHelper.SendLogsToChannel(log);
            });
    }
}
