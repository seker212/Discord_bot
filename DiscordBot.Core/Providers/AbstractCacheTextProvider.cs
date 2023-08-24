using DiscordBot.Core.Interfaces;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Abstract provider for caching text data specific to guildId
    /// </summary>
    public abstract class AbstractCacheTextProvider : ITextProvider
    {
        private readonly Dictionary<ulong, List<string>> _cache;

        public AbstractCacheTextProvider()
        {
            _cache = new();
        }

        public AbstractCacheTextProvider(Dictionary<ulong, List<string>> cache)
        {
            _cache = cache;
        }

        public void Add(ulong? guildId, string text)
        {
            var convertedGuildId = guildId ?? 0;

            if (!_cache.ContainsKey(convertedGuildId))
            {
                _cache.Add(convertedGuildId, new List<string>() { text });
            }
            else
            {
                _cache[convertedGuildId].Add(text);
            }
        }

        public void AddAll(ulong? guildId, IEnumerable<string> textsList)
        {
            foreach (var text in textsList)
            {
                Add(guildId, text);
            }
        }

        public IEnumerable<string>? GetAll(ulong? guildId)
        {
            var convertedGuildId = guildId ?? 0;

            if (!_cache.ContainsKey(convertedGuildId))
                return null;

            return _cache[convertedGuildId];
        }
    }
}
