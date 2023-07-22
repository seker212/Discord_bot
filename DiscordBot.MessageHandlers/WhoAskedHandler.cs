using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DiscordBot.MessageHandlers
{
    public class WhoAskedHandler : IMessageReceivedHandler
    {
        private const string PATTERN = @"^.*[K|k][T|t][O|o] [P|p][Y|y][T|t][A|a][L|l|Ł|ł].*$";
        private const string RESPONSE = "Ale kto pytał?";

        private readonly ILogger<WhoAskedHandler> _logger;

        public WhoAskedHandler(ILogger<WhoAskedHandler> logger)
        {
            _logger = logger;
        }

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage.Channel is SocketTextChannel &&
            Regex.IsMatch(socketMessage.Content, PATTERN);

        private async Task SendResponseAsync(SocketMessage socketMessage)
        {
            _logger.LogDebug("Sending response asking who asked?");
            var socketChannel = (socketMessage.Channel as SocketTextChannel)!;
            var mention = socketMessage.Author.Mention;

            await socketChannel.SendMessageAsync(RESPONSE + mention);
        }

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if (IsApplicable(socketMessage))
                    await SendResponseAsync(socketMessage);
            });
    }
}
