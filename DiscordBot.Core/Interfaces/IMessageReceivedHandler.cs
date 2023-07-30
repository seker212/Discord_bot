using Discord.WebSocket;

namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Exposes method to execute when any new message is received.
    /// </summary>
    public interface IMessageReceivedHandler
    {
        /// <summary>
        /// Is executed when any new message is received.
        /// </summary>
        /// <param name="socketMessage">Received message.</param>
        /// <returns>Task of method execution.</returns>
        Task Execute(SocketMessage socketMessage);
    }
}
