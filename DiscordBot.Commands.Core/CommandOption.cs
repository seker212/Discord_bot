using Discord;
using DiscordBot.Commands.Core.CommandAttributes;

namespace DiscordBot.Commands.Core
{
    public interface ICommandOption
    {
        string Description { get; }
        string Name { get; }
        CommandOptionType Type { get; }
        bool IsRequired { get; }

        bool Equals(CommandOption? other);
        bool Equals(object? obj);
        int GetHashCode();
        string ToString();
    }

    public record CommandOption : ICommandOption
    {
        public CommandOption(string name, string description, CommandOptionType type, bool isRequired)
        {
            Name = name;
            Description = description;
            Type = type;
            IsRequired = isRequired;
        }

        public CommandOption(OptionAttribute optionAttribute) : this(optionAttribute.Name, optionAttribute.Description, optionAttribute.Type, optionAttribute.IsRequired) { }

        public string Name { get; }
        public string Description { get; }
        public CommandOptionType Type { get; }
        public bool IsRequired { get; }
    }
}
