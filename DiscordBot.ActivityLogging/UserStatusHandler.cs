using Discord.WebSocket;
using DiscordBot.Core.Interfaces;


namespace DiscordBot.ActivityLogging
{
    /// <summary>
    /// Class for handling user changing status e.g from online to offline.
    /// </summary>
    public class UserStatusHandler : IUserPresenceActivityHandler
    {
        public Task Execute(SocketUser socketUser, SocketPresence oldPresence, SocketPresence newPresence)
            => Task.Run(async () =>
            {
                //TODO: This handle should mostly log presence to database rather than on channel
                // This should measure things such as online time, afk time etc.
                // No printing should be required.
                // https://github.com/seker212/Discord_bot/issues/189
            });
    }
}
