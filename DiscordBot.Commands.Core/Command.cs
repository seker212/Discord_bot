using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Core.Helpers;
using System.Globalization;

namespace DiscordBot.Commands.Core
{
    public interface ICommand
    {
        ulong? GuildId { get; }
        string Name { get; }
        IReadOnlyCollection<ICommandOption> Options { get; }
        string Description { get; }
        GuildPermission? RequiredGuildPermission { get; }

        Discord.SlashCommandBuilder CustomBuildAction(Discord.SlashCommandBuilder slashCommandBuilder);
        Task ExecuteAsync(SocketSlashCommand command);
    }

    public abstract class Command : ICommand
    {
        public ulong? GuildId { get; }
        public string Description { get; }
        public string Name { get; }
        public IReadOnlyCollection<ICommandOption> Options { get; }
        public GuildPermission? RequiredGuildPermission { get; }

        protected Command()
        {
            var attributes = GetType().GetCustomAttributes(false);
            Name = (attributes.Single(x => x.GetType() == typeof(NameAttribute)) as NameAttribute)!.Text;
            Description = (attributes.Single(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute)!.Text;
            var guildIdAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(GuildIdAttribute)) as GuildIdAttribute;
            GuildId = guildIdAttribute is null ? null : guildIdAttribute.Id;
            Options = attributes.Where(x => x.GetType() == typeof(OptionAttribute)).Select(x => new CommandOption((x as OptionAttribute)!)).ToList();
            var requiredGuildPermissionAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(RequiredPermissionAttribute)) as RequiredPermissionAttribute;
            RequiredGuildPermission = requiredGuildPermissionAttribute?.GuildPermission;
        }

        public virtual Discord.SlashCommandBuilder CustomBuildAction(Discord.SlashCommandBuilder slashCommandBuilder)
            => slashCommandBuilder;

        public abstract Task ExecuteAsync(SocketSlashCommand command);
    }
}
