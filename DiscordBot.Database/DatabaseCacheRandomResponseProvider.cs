using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using DiscordBot.Database.Repositories;

namespace DiscordBot.Database
{
    public class DatabaseCacheRandomResponseProvider : IRandomResponseProvider
    {
        private readonly IRandomResponseProvider _cacheResponseProvider;
        private readonly IRandomResponseRepository _responseRepository;

        public DatabaseCacheRandomResponseProvider(IRandomResponseRepository responseRepository)
        {
            _responseRepository = responseRepository;
            _cacheResponseProvider = new CacheRandomResponseProvider();

            var dbValues = _responseRepository.GetAll().GetAwaiter().GetResult();
            foreach ( var dbValue in dbValues )
            {
                _cacheResponseProvider.Add(Convert.ToUInt64(dbValue.GuildId), dbValue.Response);
            }
        }

        public void Add(ulong? guildId, string randomResponse)
        {
            if(randomResponse is not null)
                _responseRepository.Insert(guildId, randomResponse);

            _cacheResponseProvider.Add(guildId, randomResponse!);
        }

        public void AddAll(ulong? guildId, IEnumerable<string> randomResponses)
        {
            foreach ( var randomResponse in randomResponses )
            {
                Add(guildId, randomResponse);
            }
        }

        public IEnumerable<string>? GetAll(ulong? guildId)
            => _cacheResponseProvider.GetAll(guildId);
    }
}
