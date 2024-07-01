using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Handler for user activities
    /// </summary>
    public interface IUserPresenceActivityHandler
    {
        /// <summary>
        /// Method for handling user presence updates
        /// </summary>
        /// <param name="socketUser">User that performed and update</param>
        /// <param name="oldPresence">Old presence value</param>
        /// <param name="newPresence">New presence value</param>
        /// <returns>Activity task</returns>
        Task Execute(SocketUser socketUser, SocketPresence oldPresence, SocketPresence newPresence);
    }
}
