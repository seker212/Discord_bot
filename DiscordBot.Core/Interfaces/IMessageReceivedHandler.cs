using Discord.WebSocket;

namespace DiscordBot.Core.Interfaces
{
    public interface IMessageReceivedHandler
    {
        Task Execute(SocketMessage socketMessage);
    }
}
