﻿using Discord;

namespace DiscordBot.Core.Voice
{
    public sealed class AudioQueueEntry
    {
        public AudioQueueEntry(IVoiceChannel channel, Lazy<AudioStreamElements> audioStreamElements, Action? beforePlaying, Action? onFinish, IDictionary<string, object?>? logProperties)
        {
            Channel = channel;
            AudioStreamElements = audioStreamElements;
            BeforePlaying = beforePlaying;
            OnFinish = onFinish;
            LogProperties = logProperties ?? new Dictionary<string, object?>();
        }

        public IVoiceChannel Channel { get; }
        public Lazy<AudioStreamElements> AudioStreamElements { get; }
        public Action? BeforePlaying { get; }
        public Action? OnFinish { get; }
        public IDictionary<string, object?> LogProperties { get; }
    }
}
