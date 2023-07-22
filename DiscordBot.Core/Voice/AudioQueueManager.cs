using Discord.Audio;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Voice
{
    public class AudioQueueManager
    {
        private readonly IDictionary<ulong, Queue<AudioQueueEntry>> _guildsQueues;
        private readonly IAudioClientManager _audioClientManager;
        private readonly IDictionary<ulong, Task> _playingTaskCache;
        private readonly ILogger<AudioQueueManager> _logger;


        public int Add(ulong guildId, AudioQueueEntry audioQueueEntry)
        {
            if (_guildsQueues.ContainsKey(guildId))
                _guildsQueues[guildId].Enqueue(audioQueueEntry);
            else
                _guildsQueues.Add(guildId, new Queue<AudioQueueEntry>(new[] { audioQueueEntry }));
            
            if (_playingTaskCache.ContainsKey(guildId))
            {
                if (!_playingTaskCache[guildId].IsCompleted)
                    _playingTaskCache[guildId] = PlayQueueAsync(guildId);
            }
            else
                _playingTaskCache.Add(guildId, PlayQueueAsync(guildId));

            return _guildsQueues[guildId].Count;
        }

        public void Skip(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public void Stop(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public async Task PlayQueueAsync(ulong guildId)
        {
            using (_logger.BeginScope(new Dictionary<string, object?>() { { "GuildId", guildId } }))
                while (_guildsQueues[guildId].Any())
                {
                    var currentEntry = _guildsQueues[guildId].Dequeue();
                    IAudioClient? audioClient = null;

                    using (_logger.BeginScope(currentEntry.LogProperties))
                    {
                        if (_audioClientManager.HasActiveAudioClient(guildId))
                            if (_audioClientManager.GetGuildActiveVoiceChannel(guildId) != currentEntry.Channel) //TODO: Check if IVoiceChannel is equalable based on id
                                await _audioClientManager.LeaveChannelAsync(currentEntry.Channel);
                            else
                                audioClient = _audioClientManager.GetGuildAudioClient(guildId);
                        if (audioClient is null)
                            audioClient = await _audioClientManager.JoinChannelAsync(currentEntry.Channel);

                        var player = _audioClientManager.GetAudioPlayer(audioClient);
                        if (currentEntry.OnStart is not null)
                            currentEntry.OnStart.Invoke();
                        await player.PlayAsync(currentEntry.AudioStream.Value);
                        if (currentEntry.OnFinish is not null)
                            currentEntry.OnFinish.Invoke();
                    }
                }
        }
    }
}
