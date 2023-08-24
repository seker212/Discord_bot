using Discord;

namespace DiscordBot.Commands.Core.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequiredPermissionAttribute : Attribute
    {
        public RequiredPermissionAttribute(GuildPermission guildPermission)
        {
            GuildPermission = guildPermission;
        }

        public GuildPermission GuildPermission { get; }
    }
}
