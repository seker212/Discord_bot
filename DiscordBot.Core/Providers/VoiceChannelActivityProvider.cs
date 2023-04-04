using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Providers
{
    public interface IVoiceChannelActivityProvider
    {
        Task OnUserVoiceStateUpdate(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState);
    }

    public class VoiceChannelActivityProvider : IVoiceChannelActivityProvider
    {
        private readonly IEnumerable<IVoiceChannelActivityHandler> _handlers;
        private readonly ILogger<VoiceChannelActivityProvider> _logger;

        public VoiceChannelActivityProvider(ILogger<VoiceChannelActivityProvider> logger, IEnumerable<IVoiceChannelActivityHandler> handlers)
        {
            _logger = logger;
            _handlers = handlers;
        }

        public Task OnUserVoiceStateUpdate(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState) 
            => Task.Run(() => 
            {
                foreach(var handler in _handlers)
                    handler.Execute(socketUser, beforeVoiceState, afterVoiceState);
            });
    }
}