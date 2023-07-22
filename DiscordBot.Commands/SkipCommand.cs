using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("skip")]
    [Description("Skips currently played yt track")]
    public class SkipCommand : Command
    {
        private readonly IAudioClientManager _audioClientManager;
        private readonly ILogger<SkipCommand> _logger;
        private readonly IAudioQueueManager _audioQueueManager;

        public SkipCommand(IAudioClientManager audioClientManager, ILogger<SkipCommand> logger, IAudioQueueManager audioQueueManager)
        {
            _audioClientManager = audioClientManager;
            _logger = logger;
            _audioQueueManager = audioQueueManager;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            if (!command.GuildId.HasValue)
            {
                _logger.LogWarning("Command has been used on direct message chat");
                throw new ArgumentNullException(nameof(command.GuildId), "Command has been used on direct message chat");
            }
            if (_audioClientManager.HasActiveAudioPlayer(command.GuildId.Value))
            {
                await command.RespondAsync("Skipping track.");
                await _audioQueueManager.Skip(command.GuildId.Value);
            }
            else
                await command.RespondAsync("Guild has no active audio player.");
        }
    }
}
