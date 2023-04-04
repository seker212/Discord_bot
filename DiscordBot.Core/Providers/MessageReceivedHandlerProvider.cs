using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Providers
{
    public interface IMessageReceivedHandlerProvider
    {
        Task OnMessageReceived(SocketMessage socketMessage);
    }

    public class MessageReceivedHandlerProvider : IMessageReceivedHandlerProvider
    {
        private readonly IEnumerable<IMessageReceivedHandler> _handlers;
        private readonly ILogger<MessageReceivedHandlerProvider> _logger;

        public MessageReceivedHandlerProvider(IEnumerable<IMessageReceivedHandler> handlers, ILogger<MessageReceivedHandlerProvider> logger)
        {
            _handlers = handlers;
            _logger = logger;
        }

        public Task OnMessageReceived(SocketMessage socketMessage)
            => Task.Run(() =>
                {
                    if (!socketMessage.Author.IsBot)
                    {
                        using (_logger.BeginScope(new Dictionary<string, object>() { { "MessageID", socketMessage.Id } }))
                            foreach (var handler in _handlers)
                                handler.Execute(socketMessage);
                    }
                });
    }
}
