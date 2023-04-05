using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core.CommandAttributes;
using System.Globalization;

namespace DiscordBot.Commands.Core
{
    public interface ICommand
    {
        ulong? GuildId { get; }
        string Name { get; }
        IReadOnlyCollection<CommandOption> Options { get; }
        string Description { get; }

        SlashCommandProperties Build();
        Task ExecuteAsync(SocketSlashCommand command);
    }

    public abstract class Command : ICommand
    {
        public ulong? GuildId { get; }
        public string Description { get; }
        public string Name { get; }
        public IReadOnlyCollection<CommandOption> Options { get; }

        protected Command()
        {
            var attributes = GetType().GetCustomAttributes(false);
            Name = (attributes.Single(x => x.GetType() == typeof(NameAttribute)) as NameAttribute)!.Text;
            Description = (attributes.Single(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute)!.Text;
            var guildIdAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(GuildIdAttribute)) as GuildIdAttribute;
            GuildId = guildIdAttribute is null ? null : guildIdAttribute.Id;
            Options = attributes.Where(x => x.GetType() == typeof(OptionAttribute)).Select(x => new CommandOption((x as OptionAttribute)!)).ToList();
        }

        public SlashCommandProperties Build()
        {
            var builder = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription(Description);
            foreach (var option in Options)
                builder.AddOption(option.Name, option.Type, option.Description);
            return CustomBuildAction(builder).Build();
        }

        protected virtual SlashCommandBuilder CustomBuildAction(SlashCommandBuilder slashCommandBuilder)
            => slashCommandBuilder;

        public abstract Task ExecuteAsync(SocketSlashCommand command);
    }
}
