using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
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
        private readonly IVoiceChannelActivityProvider _voiceChannelActivityProvider;

        public BotClientRunner(DiscordSocketClient client, ITokenProvider tokenProvider, IActivityProvider activityProvider, IStartupTaskProvider startupTaskProvider, IDiscordClientLoggingProvider discordClientLoggingProvider, ISlashCommandHandlerProvider slashCommandHandlerProvider, ILogger<BotClientRunner> logger, IMessageReceivedHandlerProvider messageReceivedHandlerProvider, IVoiceChannelActivityProvider voiceChannelActivityProvider)
        {
            _client = client;
            _tokenProvider = tokenProvider;
            _activityProvider = activityProvider;
            _slashCommandHandlerProvider = slashCommandHandlerProvider;
            _logger = logger;
            _startupTaskProvider = startupTaskProvider;
            _discordClientLoggingProvider = discordClientLoggingProvider;
            _messageReceivedHandlerProvider = messageReceivedHandlerProvider;
            _voiceChannelActivityProvider = voiceChannelActivityProvider;
        }

        public async Task Run()
        {
            _logger.LogDebug("Registering client's events");
            _client.Ready += _startupTaskProvider.OnReady;
            _client.SlashCommandExecuted += _slashCommandHandlerProvider.SlashCommandHandlerAsync;
            _client.Log += _discordClientLoggingProvider.LogDiscordClientEvent;
            _client.MessageReceived += _messageReceivedHandlerProvider.OnMessageReceived;
            _client.UserVoiceStateUpdated += _voiceChannelActivityProvider.OnUserVoiceStateUpdate;
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
    }
}
