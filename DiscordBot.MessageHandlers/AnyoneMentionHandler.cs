using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;


namespace DiscordBot.MessageHandlers
{
    /// <summary>
    /// Class handling @anyone in text messages, responding with random fact
    /// </summary>
    public class AnyoneMentionHandler : IMessageReceivedHandler
    {
        private static Random _random = new Random();

        private const string SELF_PATTERN = @"@anyone";
        private readonly ILogger<AnyoneMentionHandler> _logger;
        private readonly IFactProvider _randomFactProvider;

        public AnyoneMentionHandler(ILogger<AnyoneMentionHandler> logger, IFactProvider randomFactProvider)
        {
            _logger = logger;
            _randomFactProvider = randomFactProvider;
        }

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage.Channel is SocketTextChannel &&
            Regex.IsMatch(socketMessage.Content, SELF_PATTERN);

        private async Task SendResponseAsync(SocketMessage socketMessage)
        {
            _logger.LogDebug("Sending response to @anyone");
            var socketChannel = (socketMessage.Channel as SocketTextChannel)!;

            var users = socketChannel.Users;
            var index = _random.Next(users.Count);
            var mention = users.ElementAt(index).Mention;

            var response = GetRandomFact(socketChannel.Guild.Id);

            await socketChannel.SendMessageAsync(mention + " " + response);
        }
        private string GetRandomFact(ulong guildId)
        {
            var values = _randomFactProvider.GetAll(guildId);

            if (values is null)
                return "Nothing";

            var valuesList = values.ToList();
            var index = random.Next(valuesList.Count);
            return valuesList[index];
        }

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if (IsApplicable(socketMessage))
                    await SendResponseAsync(socketMessage);
            });
    }
}
