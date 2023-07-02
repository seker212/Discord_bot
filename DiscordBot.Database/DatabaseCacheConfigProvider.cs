using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using DiscordBot.Database.Repositories;

namespace DiscordBot.Database
{
    public class DatabaseCacheConfigProvider : IConfigProvider
    {
        private readonly IConfigProvider _cacheConfigProvider;
        private readonly IConfigRepository _configRepository;

        public DatabaseCacheConfigProvider(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
            _cacheConfigProvider = new CacheConfigProvider();
            var dbConfigParams = _configRepository.GetAll().GetAwaiter().GetResult();
            foreach (var param in dbConfigParams)
                _cacheConfigProvider.SetParameter(Convert.ToUInt64(param.GuildId), param.ParameterName, param.ParameterValue);
        }

        public string? GetParameter(ulong? guildId, string parameterName)
            => _cacheConfigProvider.GetParameter(guildId, parameterName);

        public void SetParameter(ulong? guildId, string parameterName, string? value)
        {
            if (value is not null)
                _configRepository.InsertOrUpdate(guildId, parameterName, value);
            _cacheConfigProvider.SetParameter(guildId, parameterName, value);
        }
    }
}
