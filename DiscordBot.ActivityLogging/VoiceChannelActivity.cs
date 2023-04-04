using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.ActivityLogging
{
    public class VoiceChannelActivityHandler : IVoiceChannelActivityHandler
    {
        private readonly ILogger<VoiceChannelActivityHandler> _logger;

        public VoiceChannelActivityHandler(ILogger<VoiceChannelActivityHandler> logger)
        {
            _logger = logger;
        }

        public Task Execute(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState)
            => Task.Run(async () =>
            {
                
            });
    }
}