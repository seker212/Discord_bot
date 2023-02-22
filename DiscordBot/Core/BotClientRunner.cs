using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Core.Helpers;
using DiscordBot.Core.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private readonly ISlashCommandHandlerProvider _slashCommandHandlerProvider;
        private readonly ILogger<BotClientRunner> _logger;
        private readonly IDiscordLoggingHelper _discordLoggingHelper;
        private readonly ILogger<DiscordSocketClient> _discordSocketClientLoger;

        public BotClientRunner(DiscordSocketClient client, ITokenProvider tokenProvider, IActivityProvider activityProvider, ISlashCommandsManager slashCommandsRegistrant, ISlashCommandHandlerProvider slashCommandHandlerProvider, ILogger<BotClientRunner> logger, IDiscordLoggingHelper discordLoggingHelper, ILogger<DiscordSocketClient> discordSocketClientLoger)
        {
            _client = client;
            _tokenProvider = tokenProvider;
            _activityProvider = activityProvider;
            _slashCommandsRegistrant = slashCommandsRegistrant;
            _slashCommandHandlerProvider = slashCommandHandlerProvider;
            _logger = logger;
            _discordLoggingHelper = discordLoggingHelper;
            _discordSocketClientLoger = discordSocketClientLoger;
        }

        public async Task Run()
        {
            _logger.LogDebug("Registering client's events");
            _client.Ready += _slashCommandsRegistrant.RemoveUnknownCommandsAsync;
            _client.Ready += _slashCommandsRegistrant.RegisterCommandsAsync;
            _client.SlashCommandExecuted += _slashCommandHandlerProvider.SlashCommandHandler;
            _client.Log += x => _discordLoggingHelper.LogDiscordLogMessage(_discordSocketClientLoger, x);
            _client.MessageReceived += ClientOnMessageReceived;
            _logger.LogDebug("Setting bot's activity");
            await _client.SetGameAsync(_activityProvider.ActivityName, _activityProvider.TwitchStreamUrl, _activityProvider.ActivityType);
            _logger.LogDebug("Logging client");
            await _client.LoginAsync(_tokenProvider.TokenType, _tokenProvider.Token);
            _logger.LogDebug("Starting client");
            await _client.StartAsync();
            _logger.LogDebug("Client started");
            // Block this task until the program is closed.
            await Task.Delay(-1);
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
