using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.VoiceControlCommands
{
    [Name("queue")]
    [Description("Prints content of queue")]
    public class QueueCommand : Command
    {
        private readonly IAudioQueueManager _audioQueueManager;
        private readonly ILogger<QueueCommand> _logger;

        public QueueCommand(IAudioQueueManager audioQueueManager, ILogger<QueueCommand> logger)
        {
            _audioQueueManager = audioQueueManager;
            _logger = logger;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();

            var guildId = command.GuildId.Value;
            var queue = _audioQueueManager.GetQueue(guildId);

            if(queue is null || queue.Count == 0)
            {
                await command.ModifyOriginalResponseAsync(m => { m.Embed = GetEmptyQueueEmbed(); });
            }
            else
            {
                await command.ModifyOriginalResponseAsync(m => { m.Embed = GetQueueEmbed(queue); });
            }
        }

        private Embed GetEmptyQueueEmbed ()
        {
            var builder = new EmbedBuilder()
            {
                Title = "Queue is empty",
                Description = "Currently there is nothing in queue, use `/yt` or `/sound` to add elements"
            };
            return builder.Build();
        }

        private Embed GetQueueEmbed(Queue<AudioQueueEntry> queue)
        {
            var builder = new EmbedBuilder()
            {
                Title = "Queue",
            };

            var description = "";
            var number = 1;

            foreach(var entry in queue)
            {
                description += $"{number++}. {entry.Title} \r\n";
            }

            builder.WithDescription(description);

            return builder.Build();
        }
    }
}
