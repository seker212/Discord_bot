using Discord;
using DiscordBot.Commands.Core.CommandAttributes;

namespace DiscordBot.Commands.Core
{
    public interface ICommandOption
    {
        string Description { get; }
        string Name { get; }
        ApplicationCommandOptionType Type { get; }

        bool Equals(CommandOption? other);
        bool Equals(object? obj);
        int GetHashCode();
        string ToString();
    }

    public record CommandOption : ICommandOption
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
