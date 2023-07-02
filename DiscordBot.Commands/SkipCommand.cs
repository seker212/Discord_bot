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

        public SkipCommand(IAudioClientManager audioClientManager, ILogger<SkipCommand> logger)
        {
            _audioClientManager = audioClientManager;
            _logger = logger;
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
                var audioClient = _audioClientManager.GetGuildAudioClient(command.GuildId.Value);
                var audioPlayer = _audioClientManager.GetAudioPlayer(audioClient);
                if (audioPlayer.Status != AudioPlayingStatus.NotPlaying)
                    await audioPlayer.StopAsync();
            }
            else
                await command.RespondAsync("Guild has no active audio player.");
        }
    }
}
