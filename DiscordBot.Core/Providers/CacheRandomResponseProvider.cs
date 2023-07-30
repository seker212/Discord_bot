using DiscordBot.Core.Interfaces;

namespace DiscordBot.Core.Providers
{
    public class CacheRandomResponseProvider : IRandomResponseProvider
    {
        private readonly Dictionary<ulong, List<string>> _cache;

        public CacheRandomResponseProvider()
        {
            _cache = new();
        }

        public CacheRandomResponseProvider(Dictionary<ulong, List<string>> cache)
        {
            _cache = cache;
        }

        public void Add(ulong? guildId, string randomResponse)
        {
            var convertedGuildId = guildId ?? 0;

            if(!_cache.ContainsKey(convertedGuildId))
            {
                _cache.Add(convertedGuildId, new List<string>() { randomResponse });
            }
            else
            {
                _cache[convertedGuildId].Add(randomResponse);
            }
        }

        public void AddAll(ulong? guildId, IEnumerable<string> randomResponses)
        {
            foreach(var randomResponse in randomResponses)
            {
                Add(guildId, randomResponse);
            }
        }

        public List<string> GetAll(ulong? guildId)
        {
            var convertedGuildId = guildId ?? 0;
            
            if(!_cache.ContainsKey(convertedGuildId))
                return new();
            

            return _cache[convertedGuildId];
        }
    }
}
