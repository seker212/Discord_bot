using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using System.Text.RegularExpressions;

namespace DiscordBot.MessageHandlers
{
    /// <summary>
    /// Adds oof reaction to message that contains "oof"
    /// </summary>
    public class OofReactionHandler : IMessageReceivedHandler
    {
        private const string OOF_PATTERN = @"(\s|^)[oO]{2,}[fF](\s|$)";

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage.Channel is SocketTextChannel socketChannel &&
            socketChannel.Guild.Emotes.Any(x => x.Name.ToLower() == "oof") &&
            Regex.IsMatch(socketMessage.Content, OOF_PATTERN);

        private Task ApplyReactionAsync(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                var socketChannel = (socketMessage.Channel as SocketTextChannel)!;
                var emote = socketChannel.Guild.Emotes.SingleOrDefault(x => x.Name.ToLower() == "oof");
                await socketMessage.AddReactionAsync(emote);
            });

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if (IsApplicable(socketMessage))
                    await ApplyReactionAsync(socketMessage);
            });

    }
}
