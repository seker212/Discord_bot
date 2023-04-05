using Discord;

namespace DiscordBot.Commands.Core.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(string name, string description, ApplicationCommandOptionType type, bool isRequired = true)
        {
            Name = name;
            Description = description;
            Type = type;
            IsRequired = isRequired;
        }

        public string Name { get; }
        public string Description { get; }
        public ApplicationCommandOptionType Type { get; }
        public bool IsRequired { get; }
    }
}
