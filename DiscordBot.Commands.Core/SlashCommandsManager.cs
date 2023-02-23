using Discord.WebSocket;
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
            Task AddCommands(ConcurrentBag<SocketApplicationCommand> collection, Func<Task<IReadOnlyCollection<SocketApplicationCommand>>> commandsGetter)
            => Task.Run(async () =>
            {
                var commandsCollection = await commandsGetter();
                foreach (var command in commandsCollection)
                    collection.Add(command);
            });

            return Task.Run(() =>
            {
                var serverCommands = new ConcurrentBag<SocketApplicationCommand>();

                var taskCache = _client.Guilds.Select(x => AddCommands(serverCommands, () => x.GetApplicationCommandsAsync()))
                    .Append(AddCommands(serverCommands, () => _client.GetGlobalApplicationCommandsAsync()))
                    .ToArray();
                Task.WaitAll(taskCache.ToArray());
                
                var removalTasks = serverCommands
                    .Where(serverCmd => !_commands.Any(x => x.Name == serverCmd.Name && (x.GuildId is null && serverCmd.IsGlobalCommand || x.GuildId == serverCmd.Guild.Id))) //FIXME: Add proper command comparing
                    .Select(serverCmd => RemoveServerCommandAsync(serverCmd))
                    .ToArray();
                Task.WaitAll(removalTasks);
            });
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
