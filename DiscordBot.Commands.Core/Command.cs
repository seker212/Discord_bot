using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Commands.Core.CommandAttributes;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DiscordBot.Commands.Core
{
    public interface ICommand
    {
        ulong? GuildId { get; }
        string Name { get; }

        SlashCommandProperties Build();
        Task ExecuteAsync(SocketSlashCommand command);
    }

    public abstract class Command : ICommand
    {
        private readonly string _description;

        public ulong? GuildId { get; }
        public string Name { get; }

        protected Command()
        {
            var attributes = GetType().GetCustomAttributes(false);
            Name = (attributes.Single(x => x.GetType() == typeof(NameAttribute)) as NameAttribute).Text;
            _description = (attributes.Single(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute).Text;
            var guildIdAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(GuildIdAttribute)) as GuildIdAttribute;
            GuildId = guildIdAttribute is null ? null : guildIdAttribute.Id;
        }

        public SlashCommandProperties Build()
        {
            var builder = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription(_description);
            return CustomBuildAction(builder).Build();
        }

        public virtual SlashCommandBuilder CustomBuildAction(SlashCommandBuilder slashCommandBuilder)
            => slashCommandBuilder;

        public abstract Task ExecuteAsync(SocketSlashCommand command);
    }
}
