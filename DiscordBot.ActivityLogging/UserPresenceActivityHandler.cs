

using Discord.WebSocket;
using DiscordBot.Core.Interfaces;

namespace DiscordBot.ActivityLogging
{
    /// <summary>
    /// Class for handling user changing presence e.g starting playing game.
    /// </summary>
    public class UserPresenceActivityHandler : IUserPresenceActivityHandler
    {
        public Task Execute(SocketUser socketUser, SocketPresence oldPresence, SocketPresence newPresence)
            => Task.Run(async () =>
            {
            // TODO: This handler should measure user time which someone plays something or starts a new game.
            // It's quite complex.
            // Could also be logged on channels.
            // https://github.com/seker212/Discord_bot/issues/189
            });
    }
}
