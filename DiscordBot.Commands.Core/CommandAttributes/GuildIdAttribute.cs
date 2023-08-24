namespace DiscordBot.Commands.Core.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GuildIdAttribute : Attribute
    {
        public GuildIdAttribute(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }
    }
}
