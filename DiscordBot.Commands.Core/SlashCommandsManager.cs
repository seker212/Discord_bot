using Discord.WebSocket;
using DiscordBot.Commands.Core.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DiscordBot.Commands.Core
{
    /// <summary>
    /// Registers and deletes bot's slash commands
    /// </summary>
    public interface ISlashCommandsManager
    {
        Task RegisterCommandsAsync();
        Task RemoveUnknownCommandsAsync();
    }

    /// <inheritdoc cref="ISlashCommandsManager"/>
    public class SlashCommandsManager : ISlashCommandsManager
    {
        readonly DiscordSocketClient _client;
        private readonly IEnumerable<ICommand> _commands;
        private readonly ICommandComparer _commandComparer;
        private readonly ILogger<SlashCommandsManager> _logger;
        private readonly ISlashCommandBuilder _commandBuilder;

        public SlashCommandsManager(DiscordSocketClient client, IEnumerable<ICommand> commands, ILogger<SlashCommandsManager> logger, ICommandComparer commandComparer, ISlashCommandBuilder commandBuilder)
        {
            _client = client;
            _commands = commands;
            _logger = logger;
            _commandComparer = commandComparer;
            _commandBuilder = commandBuilder;
        }

        public Task RemoveUnknownCommandsAsync()
        {
            return Task.Run(() =>
            {
                var serverCommands = GetRegisteredCommandsAsync();

                var removalTasks = serverCommands
                    .Where(serverCmd => !_commands.Any(x => _commandComparer.CommandEquals(x, serverCmd)))
                    .Select(serverCmd => RemoveServerCommandAsync(serverCmd))
                    .ToArray();
                Task.WaitAll(removalTasks);
            });
        }

        private IEnumerable<SocketApplicationCommand> GetRegisteredCommandsAsync()
        {
            var serverCommands = new ConcurrentBag<SocketApplicationCommand>();

            Func<Task> GetAddCommandsFunc(Func<Task<IReadOnlyCollection<SocketApplicationCommand>>> commandsGetter)
            => async () =>
            {
                var commandsCollection = await commandsGetter();
                foreach (var command in commandsCollection)
                    serverCommands.Add(command);
            };

            var requestTasks = _client.Guilds.Select(guild => GetAddCommandsFunc(async () => await guild.GetApplicationCommandsAsync()))
                    .Append(GetAddCommandsFunc(async () => await _client.GetGlobalApplicationCommandsAsync()))
                    .Select(f => f())
                    .ToArray();
            Task.WaitAll(requestTasks);
            return serverCommands;
        }

        private Task RemoveServerCommandAsync(SocketApplicationCommand serverCommand)
        {
            return Task.Run(async () =>
            {
                _logger.LogDebug("Removing slash command {CommandName} from server", serverCommand.Name);
                await serverCommand.DeleteAsync();
                _logger.LogDebug("Removed slash command {CommandName} from server", serverCommand.Name);
            });
        }

        public Task RegisterCommandsAsync()
        {
            return Task.Run(() =>
            {
                var taskCache = _commands.Select(c => RegisterCommandAsync(c)).ToArray();
                Task.WaitAll(taskCache);
            });
        }

        private Task RegisterCommandAsync(ICommand command)
        {
            if (command.GuildId.HasValue)
                return Task.Run(async () =>
                {
                    var guild = _client.GetGuild(command.GuildId.Value);
                    _logger.LogDebug("Registering guild slash command {CommandName} for guild {GuildName}", command.Name, guild.Name);
                    await guild.CreateApplicationCommandAsync(_commandBuilder.Build(command));
                    _logger.LogDebug("Registered guild slash command {CommandName} for guild {GuildName}", command.Name, guild.Name);
                });
            else
                return Task.Run(async () =>
                {
                    _logger.LogDebug("Registering global slash command {CommandName}", command.Name);
                    await _client.CreateGlobalApplicationCommandAsync(_commandBuilder.Build(command));
                    _logger.LogDebug("Registered global slash command {CommandName}", command.Name);
                });
        }
    }
}
