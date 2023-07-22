using Discord;

namespace DiscordBot.Core.Voice
{
    public sealed class AudioQueueEntry
    {
        public AudioQueueEntry(IVoiceChannel channel, Lazy<Stream> audioStream, Action? beforePlaying, Action? onFinish, IDictionary<string, object?>? logProperties)
        {
            Channel = channel;
            AudioStream = audioStream;
            BeforePlaying = beforePlaying;
            OnFinish = onFinish;
            LogProperties = logProperties ?? new Dictionary<string, object?>();
        }

        public IVoiceChannel Channel { get; }
        public Lazy<Stream> AudioStream { get; }
        public Action? BeforePlaying { get; }
        public Action? OnFinish { get; }
        public IDictionary<string, object?> LogProperties { get; }
    }
}
