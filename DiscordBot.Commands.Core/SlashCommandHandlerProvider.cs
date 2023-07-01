using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Providers;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Core
{
    /// <inheritdoc cref="ISlashCommandHandlerProvider"/>
    public class SlashCommandHandlerProvider : ISlashCommandHandlerProvider
    {
        readonly IEnumerable<ICommand> _commands; //TODO: Change this to command provider. Use hash dict?
        private readonly ICommandComparer _commandComparer;
        private readonly ILogger<SlashCommandHandlerProvider> _logger;

        public SlashCommandHandlerProvider(IEnumerable<ICommand> commands, ICommandComparer commandComparer, ILogger<SlashCommandHandlerProvider> logger)
        {
            _commands = commands;
            _commandComparer = commandComparer;
            _logger = logger;
        }

        public Task SlashCommandHandlerAsync(SocketSlashCommand commandInput)
        {
            return Task.Run(async () => 
            {
                using (_logger.BeginScope(new Dictionary<string, object?>() { { "CommandCallID", commandInput.Id }, { "GuildId", commandInput.GuildId }, { "CommandName", commandInput.CommandName }, { "CommandArguments", commandInput.Data.Options.ToDictionary(x => x.Name, x => x.Value) } }))
                {
                    _logger.LogDebug("Recieved slash command");
                    ICommand command = null!;
                    try { command = _commands.Single(x => _commandComparer.CommandSyntaxEquals(x, commandInput)); }
                    catch (InvalidOperationException)
                    {
                        _logger.LogWarning("Command with name {CommandName} was not found. GuildId: {GuildId}", commandInput.CommandName, commandInput.GuildId);
                        await commandInput.RespondAsync($"Command with name {commandInput.CommandName} was not found");
                    }
                    try 
                    { 
                        if (commandInput.GuildId is not null && command.RequiredGuildPermission.HasValue)
                        {
                            var guildUser = commandInput.User as IGuildUser;
                            if (guildUser!.GuildPermissions.Has(command.RequiredGuildPermission.Value))
                                await command.ExecuteAsync(commandInput);
                            else
                            {
                                _logger.LogWarning("User {Username} tried to invoke command {CommandName} without guild permission {GuildPermission}", guildUser.DisplayName, command.Name, command.RequiredGuildPermission.Value);
                                await commandInput.FollowupAsync($"You don't have permission to use this command.");
                            }

                        }
                        else
                            await command.ExecuteAsync(commandInput); 
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Command {CommandName} could not be handled. Used options: {Options}", commandInput.CommandName, commandInput.Data.Options.Select(x => (x.Name, x.Value)));
                        await commandInput.RespondAsync($"Command could not be handled.");
                    }
                }
            });
        }
    }
}
