using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Core
{
    /// <summary>
    /// Provides function for handling slash commands
    /// </summary>
    public interface ISlashCommandHandlerProvider
    {
        Task SlashCommandHandlerAsync(SocketSlashCommand commandInput);
    }

    /// <inheritdoc cref="ISlashCommandHandlerProvider"/>
    public class SlashCommandHandlerProvider : ISlashCommandHandlerProvider
    {
        readonly IEnumerable<ICommand> _commands; //TODO: Change this to command provider. Use hash dict?
        private readonly ILogger<SlashCommandHandlerProvider> _logger;

        public SlashCommandHandlerProvider(IEnumerable<ICommand> commands, ILogger<SlashCommandHandlerProvider> logger)
        {
            _commands = commands;
            _logger = logger;
        }

        public Task SlashCommandHandlerAsync(SocketSlashCommand commandInput)
        {
            return Task.Run(async () => 
            {
                ICommand command = null!;
                try { command = _commands.Single(x => x.Name == commandInput.CommandName && (x.GuildId is null || x.GuildId == commandInput.GuildId)); } //FIXME: Add proper command comparing
                catch (InvalidOperationException)
                {
                    _logger.LogWarning("Command with name {CommandName} was not found. GuildId: {GuildId}", commandInput.CommandName, commandInput.GuildId);
                    await commandInput.RespondAsync($"Command with name {commandInput.CommandName} was not found");
                }
                try { 
                    if (command is not null) 
                        await command.ExecuteAsync(commandInput); 
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Command {CommandName} could not be handled. Used options: {Options}", commandInput.CommandName, commandInput.Data.Options.Select(x => (x.Name, x.Value)));
                    await commandInput.RespondAsync($"Command could not be handled.");
                }
            });
        }
    }
}
