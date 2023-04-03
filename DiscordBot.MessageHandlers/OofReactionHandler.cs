using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DiscordBot.MessageHandlers
{
    /// <summary>
    /// Adds oof reaction to message that contains "oof"
    /// </summary>
    public class OofReactionHandler : IMessageReceivedHandler
    {
        private const string OOF_PATTERN = @"(\s|^)[oO]{2,}[fF](\s|$)";
        private const string EMOTE_NAME = "oof";
        private readonly ILogger<OofReactionHandler> _logger;

        public OofReactionHandler(ILogger<OofReactionHandler> logger)
        {
            _logger = logger;
        }

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage.Channel is SocketTextChannel socketChannel &&
            socketChannel.Guild.Emotes.Any(x => x.Name.ToLower() == EMOTE_NAME) &&
            Regex.IsMatch(socketMessage.Content, OOF_PATTERN);

        private async Task ApplyReactionAsync(SocketMessage socketMessage)
        {
            _logger.LogDebug("Applying oof reaction to message");
            var socketChannel = (socketMessage.Channel as SocketTextChannel)!;
            var emote = socketChannel.Guild.Emotes.SingleOrDefault(x => x.Name.ToLower() == EMOTE_NAME);
            await socketMessage.AddReactionAsync(emote);
        }

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if (IsApplicable(socketMessage))
                    await ApplyReactionAsync(socketMessage);
            });

    }
}
