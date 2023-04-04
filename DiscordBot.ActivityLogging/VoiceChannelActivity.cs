using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using Microsoft.Extensions.Logging;

namespace DiscordBot.ActivityLogging
{
    public class VoiceChannelActivityHandler : IVoiceChannelActivityHandler
    {
        private readonly ILogger<VoiceChannelActivityHandler> _logger;
        private IChannelDataProvider _channelDataProvider;

        public VoiceChannelActivityHandler(ILogger<VoiceChannelActivityHandler> logger, IChannelDataProvider channelDataProvider)
        {
            _logger = logger;
            _channelDataProvider = channelDataProvider;
        }

        public Task Execute(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState)
            => Task.Run(async () =>
            {
                
            });
    }
}