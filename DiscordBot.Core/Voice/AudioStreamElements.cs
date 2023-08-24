namespace DiscordBot.Core.Voice
{
    public sealed class AudioStreamElements : IDisposable
    {
        private readonly IEnumerable<IDisposable> _additionalDisposables;
        
        public Stream Stream { get; }

        public AudioStreamElements(Stream stream, IEnumerable<IDisposable> additionalDisposables)
        {
            Stream = stream;
            _additionalDisposables = additionalDisposables;
        }

        public void Dispose()
        {
            try
            {
                Stream.Dispose();
            }
            finally
            {
                foreach (var disposable in _additionalDisposables)
                    disposable.Dispose();
            }
        }
    }
}
