using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using DiscordBot.Database.Repositories;


namespace DiscordBot.Database
{
    public class DatabaseCacheRandomFactProvider : IRandomFactProvider
    {
        private readonly IRandomFactProvider _cacheFactProvider;
        private readonly IRandomFactRepository _factRepository;

        public DatabaseCacheRandomFactProvider(IRandomFactRepository responseRepository)
        {
            _factRepository = responseRepository;
            _cacheFactProvider = new CacheRandomFactProvider();

            var dbValues = _factRepository.GetAll().GetAwaiter().GetResult();
            foreach (var dbValue in dbValues)
            {
                _cacheFactProvider.Add(Convert.ToUInt64(dbValue.GuildId), dbValue.Fact);
            }
        }

        public void Add(ulong? guildId, string randomFact)
        {
            if (randomFact is not null)
                _factRepository.Insert(guildId, randomFact);

            _cacheFactProvider.Add(guildId, randomFact!);
        }

        public void AddAll(ulong? guildId, IEnumerable<string> randomFacts)
        {
            foreach (var randomFact in randomFacts)
            {
                Add(guildId, randomFact);
            }
        }

        public IEnumerable<string>? GetAll(ulong? guildId)
            => _cacheFactProvider.GetAll(guildId);
    }
}
