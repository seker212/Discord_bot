using Discord;
using Discord.WebSocket;

namespace DiscordBot.Core.Helpers
{
    public static class SocketTextChannelExtension
    {
        /// <summary>
        /// Checks if channel is a whitelist channel.
        /// </summary>
        /// <param name="socketTextChannel">Checking channel.</param>
        /// <returns>True if channel uses whitelist.</returns>
        public static bool IsWhitelisted(this SocketTextChannel socketTextChannel)
            => socketTextChannel.PermissionOverwrites.Any(x => x.Permissions.ViewChannel == PermValue.Deny && x.TargetId == socketTextChannel.Guild.EveryoneRole.Id);
    }
}
