using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("stop")]
    [Description("Stops playing and clears the queue")]
    public class StopCommand : Command
    {
        private readonly ILogger<StopCommand> _logger;
        private readonly IAudioQueueManager _audioQueueManager;
        private readonly IAudioClientManager _audioClientManager;

        public StopCommand(ILogger<StopCommand> logger, IAudioQueueManager audioQueueManager, IAudioClientManager audioClientManager)
        {
            _logger = logger;
            _audioQueueManager = audioQueueManager;
            _audioClientManager = audioClientManager;
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
                await command.RespondAsync("Stopping player and cleaning queue.");
                await _audioQueueManager.Stop(command.GuildId.Value);
            }
            else
                await command.RespondAsync("Guild has no active audio player.");
        }
    }
}
