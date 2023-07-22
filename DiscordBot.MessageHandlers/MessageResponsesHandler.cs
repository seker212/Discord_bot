using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using DiscordBot.MessageHandlers.Helpers;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DiscordBot.MessageHandlers
{
    public class MessageResponsesHandler : IMessageReceivedHandler
    {
        private readonly IDiscordClient _client;
        private readonly ILogger<MessageResponsesHandler> _logger;
        private readonly IRegexResponseHelper _regexResponseHelper;

        public MessageResponsesHandler(ILogger<MessageResponsesHandler> logger, IDiscordClient client, IRegexResponseHelper regexResponseHelper)
        {
            _logger = logger;
            _client = client;
            _regexResponseHelper = regexResponseHelper;
        }

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage.Channel is SocketTextChannel &&
            (socketMessage.MentionedUsers.Any(user => _client.CurrentUser.Mention.Equals(user.Mention)) ||
            socketMessage.MentionedEveryone);

        private async Task SendResponseAsync(SocketMessage socketMessage)
        {
            var socketChannel = (socketMessage.Channel as SocketTextChannel)!;
            var mention = socketMessage.Author.Mention;
            var text = socketMessage.Content.ToLower();

            if(_regexResponseHelper.IsMatch(text))
            {
                _logger.LogDebug("Sending response from regex values");

                await socketChannel.SendMessageAsync(_regexResponseHelper.GetResponse(text) + mention);
            }        
            else if (Regex.IsMatch(text, @"[\\?] .*") || Regex.IsMatch(text, @".*[\\?]"))
            {
                _logger.LogDebug("Adding ? emoji to a message");

                var emote = Emoji.Parse(":question:");

                await socketMessage.AddReactionAsync(emote);
            } 
            else
            {
                _logger.LogDebug("Sending random response");

                var randomResponse = "Here add random response";

                await socketChannel.SendMessageAsync(randomResponse + " " + mention);
            }
        }

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if (IsApplicable(socketMessage))
                    await SendResponseAsync(socketMessage);
            });
    }
}
