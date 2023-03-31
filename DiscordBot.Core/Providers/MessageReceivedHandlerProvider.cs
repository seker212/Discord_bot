using Discord.WebSocket;
using DiscordBot.Core.MessageReceivedHandlers;

namespace DiscordBot.Core.Providers
{
    public interface IMessageReceivedHandlerProvider
    {
        Task OnMessageReceived(SocketMessage socketMessage);
    }

    public class MessageReceivedHandlerProvider : IMessageReceivedHandlerProvider
    {
        private readonly IEnumerable<IMessageReceivedHandler> _handlers;

        public MessageReceivedHandlerProvider(IEnumerable<IMessageReceivedHandler> handlers)
        {
            _handlers = handlers;
        }

        public Task OnMessageReceived(SocketMessage socketMessage)
            => Task.Run(() =>
                {
                    if (!socketMessage.Author.IsBot)
                        Task.WaitAll(_handlers.Select(x => x.Execute(socketMessage)).ToArray());
                });
    }
}
