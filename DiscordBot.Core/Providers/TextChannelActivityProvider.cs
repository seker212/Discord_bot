using Discord;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provider of text activites
    /// </summary>
    public interface ITextChannelActivityProvider
    {
        /// <summary>
        /// Method for providing deleted messages.
        /// </summary>
        /// <param name="message">Cached deleted message and it's id</param>
        /// <param name="channel">Cached channel that activity took place and it's id</param>
        /// <returns>Activity task</returns>
        Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel);
    }

    /// <inheritdoc cref="ITextChannelActivityProvider"/>
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
