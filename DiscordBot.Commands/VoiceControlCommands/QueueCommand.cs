using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.VoiceControlCommands
{
    /// <summary>
    /// Class for handling queue command.
    /// Queue command print contents of audio queue, sounds or videos that will be played.
    /// </summary>
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
            var guildId = command.GuildId.Value;
            var queue = _audioQueueManager.GetQueue(guildId);

            if(queue is null || queue.Count == 0)
            {
                await command.RespondAsync(embed: GetEmptyQueueEmbed());
            }
            else
            {
                await command.RespondAsync(embed: GetQueueEmbed(queue));
            }
        }

        /// <summary>
        /// Method for getting formatted embed where queue is empty.
        /// </summary>
        /// <returns>Built embed</returns>
        private Embed GetEmptyQueueEmbed()
        {
            var builder = new EmbedBuilder()
            {
                Title = "Queue is empty",
                Description = "Currently there is nothing in queue, use `/yt` or `/sound` to add elements"
            };
            return builder.Build();
        }

        /// <summary>
        /// Method for getting formatted embed with all queued element printed in order with numbers.
        /// </summary>
        /// <param name="queue">Queue of audio element that will be used in embed</param>
        /// <returns>Built embed</returns>
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
