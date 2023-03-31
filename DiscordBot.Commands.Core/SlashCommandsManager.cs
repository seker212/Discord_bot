using Discord.WebSocket;
using DiscordBot.Core.Helpers;
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
        private readonly ILogger<SlashCommandsManager> _logger;

        public SlashCommandsManager(DiscordSocketClient client, IEnumerable<ICommand> commands, ILogger<SlashCommandsManager> logger)
        {
            _client = client;
            _commands = commands;
            _logger = logger;
        }

        public Task RemoveUnknownCommandsAsync()
        {
            return Task.Run(() =>
            {
                var serverCommands = GetRegisteredCommands();

                var removalTasks = serverCommands
                    .Where(serverCmd => !_commands.Any(x => x.Name == serverCmd.Name && (x.GuildId is null && serverCmd.IsGlobalCommand || x.GuildId == serverCmd.Guild.Id))) //FIXME: Add proper command comparing
                    .Select(serverCmd => new Task(async () => await RemoveServerCommandAsync(serverCmd)));
                MultipleTaskRunner.RunTasksAsync(removalTasks);
            });
        }

        private IEnumerable<SocketApplicationCommand> GetRegisteredCommands()
        {
            var serverCommands = new ConcurrentBag<SocketApplicationCommand>();
            
            Task GetAddCommandsTask(Func<Task<IReadOnlyCollection<SocketApplicationCommand>>> commandsGetter)
            => new(async () =>
            {
                var commandsCollection = await commandsGetter();
                foreach (var command in commandsCollection)
                    serverCommands.Add(command);
            });

            var requestTasks = _client.Guilds.Select(guild => GetAddCommandsTask(() => guild.GetApplicationCommandsAsync()))
                    .Append(GetAddCommandsTask(() => _client.GetGlobalApplicationCommandsAsync()));
            
            MultipleTaskRunner.RunTasksAsync(requestTasks);
            
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
                    await guild.CreateApplicationCommandAsync(command.Build());
                    _logger.LogDebug("Registered guild slash command {CommandName} for guild {GuildName}", command.Name, guild.Name);
                });
            else
                return Task.Run(async () =>
                {
                    _logger.LogDebug("Registering global slash command {CommandName}", command.Name);
                    await _client.CreateGlobalApplicationCommandAsync(command.Build());
                    _logger.LogDebug("Registered global slash command {CommandName}", command.Name);
                });
        }
    }
}
