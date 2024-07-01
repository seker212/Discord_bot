using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provider of voice activites
    /// </summary>
    public interface IVoiceChannelActivityProvider
    {
        /// <summary>
        /// Method for handling user voice channel changes
        /// </summary>
        /// <param name="socketUser">User that joined/changed/left voice channel</param>
        /// <param name="beforeVoiceState">State before action was triggered</param>
        /// <param name="afterVoiceState">State after action was triggered</param>
        /// <returns>Activity task</returns>
        Task OnUserVoiceStateUpdate(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState);
        Task OnVoiceChannelStatusUpdated(Cacheable<SocketVoiceChannel, ulong> channel, string oldStatus, string newStatus);
    }

    /// <inheritdoc cref="IVoiceChannelActivityProvider"/>
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
                    handler.ExecuteVoiceUpdate(socketUser, beforeVoiceState, afterVoiceState);
            });

        public Task OnVoiceChannelStatusUpdated(Cacheable<SocketVoiceChannel, ulong> channel, string oldStatus, string newStatus)
            => Task.Run(() =>
            {
                foreach (var handler in _handlers)
                    handler.ExecuteVoiceChannelStatusUpdate(channel, oldStatus, newStatus);
            });
    }
}