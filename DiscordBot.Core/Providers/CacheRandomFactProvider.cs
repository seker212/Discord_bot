using DiscordBot.Core.Interfaces;


namespace DiscordBot.Core.Providers
{
    public class CacheRandomFactProvider : IRandomFactProvider
    {
        private readonly Dictionary<ulong, List<string>> _cache;

        public CacheRandomFactProvider()
        {
            _cache = new();
        }

        public CacheRandomFactProvider(Dictionary<ulong, List<string>> cache)
        {
            _cache = cache;
        }

        public void Add(ulong? guildId, string randomFact)
        {
            var convertedGuildId = guildId ?? 0;

            if (!_cache.ContainsKey(convertedGuildId))
            {
                _cache.Add(convertedGuildId, new List<string>() { randomFact });
            }
            else
            {
                _cache[convertedGuildId].Add(randomFact);
            }
        }

        public void AddAll(ulong? guildId, IEnumerable<string> randomFacts)
        {
            foreach (var randomResponse in randomFacts)
            {
                Add(guildId, randomResponse);
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
