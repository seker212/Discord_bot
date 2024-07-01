using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provider for user presence and activity changes
    /// </summary>
    public interface IUserPresenceActivityProvider
    {
        /// <summary>
        /// Method for handling user presence updates
        /// </summary>
        /// <param name="socketUser">User that performed and update</param>
        /// <param name="oldPresence">Old presence value</param>
        /// <param name="newPresence">New presence value</param>
        /// <returns></returns>
        Task OnPresenceUpdated(SocketUser socketUser, SocketPresence oldPresence, SocketPresence newPresence);
    }

    /// <inheritdoc cref="IUserPresenceActivityProvider"/>
    public class UserPresenceActivityProvider : IUserPresenceActivityProvider
    {
        private readonly IEnumerable<IUserPresenceActivityHandler> _handlers;
        private readonly ILogger<UserPresenceActivityProvider> _logger;

        public UserPresenceActivityProvider(ILogger<UserPresenceActivityProvider> logger, IEnumerable<IUserPresenceActivityHandler> handlers)
        {
            _logger = logger;
            _handlers = handlers;
        }

        public Task OnPresenceUpdated(SocketUser socketUser, SocketPresence oldPresence, SocketPresence newPresence)
            => Task.Run(() =>
            {
                foreach (var handler in _handlers)
                    handler.Execute(socketUser, oldPresence, newPresence);
            });
    }
}
