using Discord.WebSocket;
using Discord;

namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Handler for text channel activities
    /// </summary>
    public interface ITextChannelActivityHandler
    {
        /// <summary>
        /// Method for handling deleted messages.
        /// </summary>
        /// <param name="message">Cached deleted message and it's id</param>
        /// <param name="channel">Cached channel that activity took place and it's id</param>
        /// <returns>Activity task</returns>
        Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel);
    }
}
