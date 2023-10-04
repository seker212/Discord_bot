using Discord;
using Discord.Audio;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DiscordBot.Core.Voice
{
    public interface IAudioQueueManager
    {
        int Add(ulong guildId, AudioQueueEntry audioQueueEntry);
        int GetQueueCount(ulong guildId);
        Task SkipAsync(ulong guildId);
        Task StopAsync(ulong guildId);
    }

    public class AudioQueueManager : IAudioQueueManager
    {
        private readonly IDictionary<ulong, Queue<AudioQueueEntry>> _guildsQueues;
        private readonly IAudioClientManager _audioClientManager;
        private readonly IDictionary<ulong, Task> _playingTaskCache;
        private readonly ILogger<AudioQueueManager> _logger;

        public AudioQueueManager(IAudioClientManager audioClientManager, ILogger<AudioQueueManager> logger)
        {
            _guildsQueues = new ConcurrentDictionary<ulong, Queue<AudioQueueEntry>>();
            _audioClientManager = audioClientManager;
            _playingTaskCache = new ConcurrentDictionary<ulong, Task>();
            _logger = logger;
        }

        public int Add(ulong guildId, AudioQueueEntry audioQueueEntry)
        {
            if (_guildsQueues.ContainsKey(guildId))
                _guildsQueues[guildId].Enqueue(audioQueueEntry);
            else
                _guildsQueues.Add(guildId, new Queue<AudioQueueEntry>(new[] { audioQueueEntry }));

            if (_playingTaskCache.ContainsKey(guildId))
            {
                if (_playingTaskCache[guildId].IsCompleted)
                    _playingTaskCache[guildId] = PlayQueueAsync(guildId);
            }
            else
                _playingTaskCache.Add(guildId, PlayQueueAsync(guildId));

            return _guildsQueues[guildId].Count;
        }

        public async Task SkipAsync(ulong guildId)
        {
            if (_audioClientManager.HasActiveAudioPlayer(guildId))
            {
                var audioClient = _audioClientManager.GetGuildAudioClient(guildId);
                var audioPlayer = _audioClientManager.GetAudioPlayer(audioClient);
                if (audioPlayer.Status != AudioPlayingStatus.NotPlaying)
                    await audioPlayer.StopAsync();
            }
        }

        public async Task StopAsync(ulong guildId)
        {
            if (_guildsQueues.ContainsKey(guildId))
            {
                _logger.LogDebug("Clearing audio queue");
                _guildsQueues[guildId].Clear();
            }
            await SkipAsync(guildId);
        }

        public int GetQueueCount(ulong guildId) => _guildsQueues.ContainsKey(guildId) ? _guildsQueues[guildId].Count : 0;

        private async Task PlayQueueAsync(ulong guildId)
        {
            using (_logger.BeginScope(new Dictionary<string, object?>() { { "GuildId", guildId } }))
            {
                _logger.LogDebug("Starting playing queue for the guild");
                while (_guildsQueues[guildId].Any())
                {
                    var currentEntry = _guildsQueues[guildId].Peek();
                    IAudioClient? audioClient = null;

                    using (_logger.BeginScope(currentEntry.LogProperties))
                    {
                        if (_audioClientManager.HasActiveAudioClient(guildId))
                            if (_audioClientManager.GetGuildActiveVoiceChannel(guildId) != currentEntry.Channel)
                            {
                                _logger.LogDebug("Leaving previous channel");
                                await _audioClientManager.LeaveChannelAsync(_audioClientManager.GetGuildActiveVoiceChannel(guildId));
                                audioClient = null;
                            }
                            else
                                audioClient = _audioClientManager.GetGuildAudioClient(guildId);
                        if (audioClient is null)
                        {
                            _logger.LogDebug("Connecting to channel {VoiceChannel}", currentEntry.Channel.Name);
                            audioClient = await _audioClientManager.JoinChannelAsync(currentEntry.Channel);
                        }

                        var player = _audioClientManager.GetAudioPlayer(audioClient);
                        if (currentEntry.BeforePlaying is not null)
                            currentEntry.BeforePlaying.Invoke();
                        try 
                        { 
                            await player.PlayAsync(currentEntry.AudioStreamElements.Value.Stream); 
                        }
                        finally 
                        {
                            _logger.LogDebug("Disposong current AudioStreamElements");
                            currentEntry.AudioStreamElements.Value.Dispose(); 
                        }
                        if (currentEntry.OnFinish is not null)
                            currentEntry.OnFinish.Invoke();
                    }
                    _logger.LogDebug("Removig played track from queue");
                    if (_guildsQueues[guildId].Count > 0)
                        _guildsQueues[guildId].Dequeue();
                    _logger.LogDebug("Tracks left in queue: {queueCount}", _guildsQueues[guildId].Count);
                }
                _logger.LogDebug("Leaving channel");
                await _audioClientManager.LeaveChannelAsync(_audioClientManager.GetGuildActiveVoiceChannel(guildId));
                _logger.LogDebug("Finished playing queue for the guild");
            }
        }
    }
}
