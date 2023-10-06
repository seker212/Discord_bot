using Discord.WebSocket;
using Discord;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Providers
{
    public interface ITextChannelActivityProvider
    {
        Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel);
    }

    public class TextChannelActivityProvider : ITextChannelActivityProvider
    {
        private readonly IEnumerable<ITextChannelActivityHandler> _handlers;
        private readonly ILogger<TextChannelActivityProvider> _logger;

        public TextChannelActivityProvider(ILogger<TextChannelActivityProvider> logger, IEnumerable<ITextChannelActivityHandler> handlers)
        {
            _logger = logger;
            _handlers = handlers;
        }

        public Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
            => Task.Run(() =>
            {
                foreach (var handler in _handlers)
                    handler.OnMessageDeleted(message, channel);
            });
    }
}
