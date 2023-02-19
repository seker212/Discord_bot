using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SlashCommandsManager(DiscordSocketClient client, IEnumerable<ICommand> commands)
        {
            _client = client;
            _commands = commands;
        }

        public Task RemoveUnknownCommandsAsync()
        {
            return Task.Run(() =>
            {
                var commands = new ConcurrentBag<SocketApplicationCommand>();

                Task AddCommands(ConcurrentBag<SocketApplicationCommand> collection, Func<Task<IReadOnlyCollection<SocketApplicationCommand>>> commandsGetter)
                => Task.Run(async () =>
                {
                    var commandsCollection = await commandsGetter();
                    foreach (var command in commandsCollection)
                        collection.Add(command);
                });

                var taskCache = new List<Task>
                {
                    AddCommands(commands, () => _client.GetGlobalApplicationCommandsAsync())
                };
                foreach (var guild in _client.Guilds)
                    taskCache.Add(AddCommands(commands, () => guild.GetApplicationCommandsAsync()));
                Task.WaitAll(taskCache.ToArray());
                foreach (var command in commands)
                {
                    if (!_commands.Any(x => x.Name == command.Name && (x.GuildId is null && command.IsGlobalCommand || x.GuildId == command.Guild.Id)))
                        command.DeleteAsync();
                }
            });
        }

        public Task RegisterCommandsAsync()
        {
            return Task.Run(() =>
            {
                var taskCache = new List<Task>();
                foreach (var command in _commands)
                    taskCache.Add(RegisterCommandAsync(command));
                Task.WaitAll(taskCache.ToArray());
            });
        }

        private Task RegisterCommandAsync(ICommand command)
        {
            if (command.GuildId.HasValue)
                return _client.GetGuild(command.GuildId.Value).CreateApplicationCommandAsync(command.Build());
            else
                return _client.CreateGlobalApplicationCommandAsync(command.Build());
        }
    }
}
