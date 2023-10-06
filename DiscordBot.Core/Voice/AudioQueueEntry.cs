using Discord;
using Discord.WebSocket;

namespace DiscordBot.Core.Voice
{
    public sealed class AudioQueueEntry
    {
        public AudioQueueEntry(IVoiceChannel channel, Lazy<AudioStreamElements> audioStreamElements, Action? beforePlaying, Action? onFinish, IDictionary<string, object?>? logProperties, string title)
        {
            Channel = channel;
            AudioStreamElements = audioStreamElements;
            BeforePlaying = beforePlaying;
            OnFinish = onFinish;
            LogProperties = logProperties ?? new Dictionary<string, object?>();
            Title = title;
        }

        public IVoiceChannel Channel { get; }
        public Lazy<AudioStreamElements> AudioStreamElements { get; }
        public Action? BeforePlaying { get; }
        public Action? OnFinish { get; }
        public IDictionary<string, object?> LogProperties { get; }
        public string Title { get; }
    }
}
