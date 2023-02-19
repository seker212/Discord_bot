using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class BotClientRunner
    {
        private readonly DiscordSocketClient _client;
        private readonly ITokenProvider _tokenProvider;
        private readonly IActivityProvider _activityProvider;
        private readonly ISlashCommandsManager _slashCommandsRegistrant;
        private readonly IEnumerable<ICommand> _commands;
        private readonly ISlashCommandHandlerProvider _slashCommandHandlerProvider;

        public BotClientRunner(DiscordSocketClient client, ITokenProvider tokenProvider, IActivityProvider activityProvider, ISlashCommandsManager slashCommandsRegistrant, IEnumerable<ICommand> commands, ISlashCommandHandlerProvider slashCommandHandlerProvider)
        {
            _client = client;
            _tokenProvider = tokenProvider;
            _activityProvider = activityProvider;
            _slashCommandsRegistrant = slashCommandsRegistrant;
            _commands = commands;
            _slashCommandHandlerProvider = slashCommandHandlerProvider;
        }

        public async Task Run()
        {
            _client.Ready += _slashCommandsRegistrant.RemoveUnknownCommandsAsync;
            _client.Ready += _slashCommandsRegistrant.RegisterCommandsAsync;
            _client.SlashCommandExecuted += _slashCommandHandlerProvider.SlashCommandHandler;
            _client.Log += Log;
            _client.MessageReceived += ClientOnMessageReceived;
            await _client.SetGameAsync(_activityProvider.ActivityName, _activityProvider.TwitchStreamUrl, _activityProvider.ActivityType);
            await _client.LoginAsync(_tokenProvider.TokenType, _tokenProvider.Token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) //TODO: Add proper logging
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task ClientOnMessageReceived(SocketMessage socketMessage) //TODO: Create separate structure for those events
        {
            await Task.Run(() =>
            {
                if (!socketMessage.Author.IsBot && socketMessage.Channel is SocketTextChannel socketChannel)
                {
                    var guild = socketChannel.Guild;
                    var oof = guild.Emotes.SingleOrDefault(x => x.Name.ToLower() == "oof");
                    if (oof is not null)
                    {
                        var message = socketMessage.Content;

                        string pattern = @"^[^a-zA-Z0-9]*[o]+of[^a-zA-Z0-9]*$"; //TODO: Review this regex
                        Regex rgx = new Regex(pattern);
                        foreach (Match match in rgx.Matches(message))
                        {
                            socketMessage.AddReactionAsync(oof);
                            Console.WriteLine(socketMessage.Content);

                            socketChannel.SendMessageAsync(message);
                            break;
                        }

                        socketChannel.SendMessageAsync(socketMessage.Content + socketMessage.Author + "asdasd");
                    }
                }
            });
        }
    }
}
