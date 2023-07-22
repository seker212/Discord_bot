using Discord;
using Discord.Audio;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Core.Voice
{
    public interface IAudioQueueManager
    {
        int Add(ulong guildId, AudioQueueEntry audioQueueEntry);
        Task Skip(ulong guildId);
        Task Stop(ulong guildId);
    }

    public class AudioQueueManager : IAudioQueueManager
    {
        private readonly IDictionary<ulong, Queue<AudioQueueEntry>> _guildsQueues;
        private readonly IAudioClientManager _audioClientManager;
        private readonly IDictionary<ulong, Task> _playingTaskCache;
        private readonly ILogger<AudioQueueManager> _logger;

        public AudioQueueManager(IAudioClientManager audioClientManager, ILogger<AudioQueueManager> logger)
        {
            _guildsQueues = new Dictionary<ulong, Queue<AudioQueueEntry>>();
            _audioClientManager = audioClientManager;
            _playingTaskCache = new Dictionary<ulong, Task>();
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
                if (!_playingTaskCache[guildId].IsCompleted)
                    _playingTaskCache[guildId] = PlayQueueAsync(guildId);
            }
            else
                _playingTaskCache.Add(guildId, PlayQueueAsync(guildId));

            return _guildsQueues[guildId].Count;
        }

        public async Task Skip(ulong guildId)
        {
            if (_audioClientManager.HasActiveAudioPlayer(guildId))
            {
                var audioClient = _audioClientManager.GetGuildAudioClient(guildId);
                var audioPlayer = _audioClientManager.GetAudioPlayer(audioClient);
                if (audioPlayer.Status != AudioPlayingStatus.NotPlaying)
                    await audioPlayer.StopAsync();
            }
        }

        public async Task Stop(ulong guildId)
        {
            if (_guildsQueues.ContainsKey(guildId))
                _guildsQueues[guildId].Clear();
            await Skip(guildId);
        }

        private async Task PlayQueueAsync(ulong guildId)
        {
            using (_logger.BeginScope(new Dictionary<string, object?>() { { "GuildId", guildId } }))
            {
                while (_guildsQueues[guildId].Any())
                {
                    _logger.LogDebug("Starting playing queue for the guild");
                    var currentEntry = _guildsQueues[guildId].Dequeue();
                    IAudioClient? audioClient = null;

                    using (_logger.BeginScope(currentEntry.LogProperties))
                    {
                        if (_audioClientManager.HasActiveAudioClient(guildId))
                            if (_audioClientManager.GetGuildActiveVoiceChannel(guildId) != currentEntry.Channel) //TODO: Check if IVoiceChannel is equalable based on id
                            {
                                _logger.LogDebug("Leaving previous channel");
                                await _audioClientManager.LeaveChannelAsync(currentEntry.Channel);
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
                        await player.PlayAsync(currentEntry.AudioStreamElements.Value.Stream);
                        if (currentEntry.OnFinish is not null)
                            currentEntry.OnFinish.Invoke();
                        currentEntry.AudioStreamElements.Value.Dispose();
                    }
                }
                _logger.LogDebug("Leaving previous channel");
                await _audioClientManager.LeaveChannelAsync(_audioClientManager.GetGuildActiveVoiceChannel(guildId));
                _logger.LogDebug("Finished playing queue for the guild");
            }
        }
    }
}
