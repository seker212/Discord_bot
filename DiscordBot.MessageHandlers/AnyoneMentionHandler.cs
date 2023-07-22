using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;


namespace DiscordBot.MessageHandlers
{
    public class AnyoneMentionHandler : IMessageReceivedHandler
    {
        private static Random random = new Random();

        private const string SELF_PATTERN = @"@anyone";
        private readonly ILogger<AnyoneMentionHandler> _logger;

        public AnyoneMentionHandler(ILogger<AnyoneMentionHandler> logger)
        {
            _logger = logger;
        }

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage.Channel is SocketTextChannel &&
            Regex.IsMatch(socketMessage.Content, SELF_PATTERN);

        private async Task SendResponseAsync(SocketMessage socketMessage)
        {
            _logger.LogDebug("Sending response to @anyone");
            var socketChannel = (socketMessage.Channel as SocketTextChannel)!;

            var users = socketChannel.Users;
            var index = random.Next(users.Count);
            var mention = users.ElementAt(index).Mention;

            var response = "<tu random fact>";

            await socketChannel.SendMessageAsync(mention + " " + response);
        }

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if (IsApplicable(socketMessage))
                    await SendResponseAsync(socketMessage);
            });
    }
}
