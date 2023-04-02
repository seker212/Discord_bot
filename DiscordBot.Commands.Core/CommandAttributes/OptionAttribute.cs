using Discord;

namespace DiscordBot.Commands.Core.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(string name, string description, ApplicationCommandOptionType type)
        {
            Name = name;
            Description = description;
            Type = type;
        }

        public string Name { get; }
        public string Description { get; }
        public ApplicationCommandOptionType Type { get; }
    }
}
