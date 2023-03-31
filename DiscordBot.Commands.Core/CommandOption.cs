using Discord;
using DiscordBot.Commands.Core.CommandAttributes;

namespace DiscordBot.Commands.Core
{
    public record CommandOption
    {
        public CommandOption(string name, string description, ApplicationCommandOptionType type)
        {
            Name = name;
            Description = description;
            Type = type;
        }

        public CommandOption(OptionAttribute optionAttribute) : this(optionAttribute.Name, optionAttribute.Description, optionAttribute.Type) { }

        public string Name { get; }
        public string Description { get; }
        public ApplicationCommandOptionType Type { get; }
    }
}
