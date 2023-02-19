using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.Core
{
    /// <summary>
    /// Provides function for handling slash commands
    /// </summary>
    public interface ISlashCommandHandlerProvider
    {
        Task SlashCommandHandler(SocketSlashCommand command);
    }

    /// <inheritdoc cref="ISlashCommandHandlerProvider"/>
    public class SlashCommandHandlerProvider : ISlashCommandHandlerProvider
    {
        readonly IEnumerable<ICommand> _commands; //TODO: Change this to hash dict?

        public SlashCommandHandlerProvider(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            try
            {
                var commandObj = _commands.Single(x => x.Name == command.CommandName && (x.GuildId is null || x.GuildId == command.GuildId));
                await commandObj.ExecuteAsync(command);
            }
            catch (InvalidOperationException)
            {
                await command.RespondAsync($"Command with name {command.CommandName} was not found");
            }
            catch (Exception ex)
            {
                await command.RespondAsync($"Command handling threw exception: {ex.Message}");
            }
        }
    }
}
