using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DiscordBot.MessageHandlers
{
    /// <summary>
    /// Class for handling `@self` in text messages and responding to it
    /// </summary>
    public class SelfMentionHandler : IMessageReceivedHandler
    {
        private const string SELF_PATTERN = @"@self";
        private const string RESPONSE = " ( ͡° ͜ʖ ͡°)";
        private readonly ILogger<SelfMentionHandler> _logger;

        public SelfMentionHandler(ILogger<SelfMentionHandler> logger) 
        { 
            _logger = logger;
        }

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage.Channel is SocketTextChannel &&
            Regex.IsMatch(socketMessage.Content, SELF_PATTERN);

        private async Task SendResponseAsync(SocketMessage socketMessage)
        {
            _logger.LogDebug("Sending response to @self");
            var mention = socketMessage.Author.Mention;

            await (socketMessage as IUserMessage)!.ReplyAsync(mention + RESPONSE);
        }

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if(IsApplicable(socketMessage))
                    await SendResponseAsync(socketMessage);
            });
    }
}
