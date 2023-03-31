using Discord.WebSocket;

namespace DiscordBot.Core.MessageReceivedHandlers
{
    public interface IMessageReceivedHandler
    {
        Task Execute(SocketMessage socketMessage);
    }
}
