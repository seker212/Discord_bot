using Discord.WebSocket;
using DiscordBot.Core.Providers;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DiscordBot.Core
{
    public class BotClientRunner
    {
        private readonly DiscordSocketClient _client;
        private readonly ITokenProvider _tokenProvider;
        private readonly IActivityProvider _activityProvider;
        private readonly ISlashCommandHandlerProvider _slashCommandHandlerProvider;
        private readonly ILogger<BotClientRunner> _logger;
        private readonly IStartupTaskProvider _startupTaskProvider;
        private readonly IDiscordClientLoggingProvider _discordClientLoggingProvider;
        private readonly IMessageReceivedHandlerProvider _messageReceivedHandlerProvider;

        public BotClientRunner(DiscordSocketClient client, ITokenProvider tokenProvider, IActivityProvider activityProvider, IStartupTaskProvider startupTaskProvider, IDiscordClientLoggingProvider discordClientLoggingProvider, ISlashCommandHandlerProvider slashCommandHandlerProvider, ILogger<BotClientRunner> logger, IMessageReceivedHandlerProvider messageReceivedHandlerProvider)
        {
            _client = client;
            _tokenProvider = tokenProvider;
            _activityProvider = activityProvider;
            _slashCommandHandlerProvider = slashCommandHandlerProvider;
            _logger = logger;
            _startupTaskProvider = startupTaskProvider;
            _discordClientLoggingProvider = discordClientLoggingProvider;
            _messageReceivedHandlerProvider = messageReceivedHandlerProvider;
        }

        public async Task Run()
        {
            _logger.LogDebug("Registering client's events");
            _client.Ready += _startupTaskProvider.OnReady;
            _client.SlashCommandExecuted += _slashCommandHandlerProvider.SlashCommandHandlerAsync;
            _client.Log += _discordClientLoggingProvider.LogDiscordClientEvent;
            _client.MessageReceived += _messageReceivedHandlerProvider.OnMessageReceived;
            _logger.LogDebug("Client's events registered");
            _logger.LogDebug("Setting bot's activity...");
            await _client.SetGameAsync(_activityProvider.ActivityName, _activityProvider.TwitchStreamUrl, _activityProvider.ActivityType);
            _logger.LogDebug("Bot's activity set");
            _logger.LogDebug("Logging client...");
            await _client.LoginAsync(_tokenProvider.TokenType, _tokenProvider.Token);
            _logger.LogDebug("Client logged in");
            _logger.LogDebug("Starting client...");
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
