using Discord.WebSocket;
using Discord;

namespace DiscordBot.Core.Interfaces
{
    public interface ITextChannelActivityHandler
    {
        Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel);
    }
}
