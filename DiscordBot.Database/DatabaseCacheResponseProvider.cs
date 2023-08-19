using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using DiscordBot.Database.Repositories;
using DiscordBot.Database.TablesModels;

namespace DiscordBot.Database
{
    /// <summary>
    /// Database cache provider specific for Responses
    /// </summary>
    public class DatabaseCacheResponseProvider : AbstractDatabaseCacheTextProvider<Responses>, IResponseProvider
    {
        public DatabaseCacheResponseProvider(IResponseRepository textRepository) : base(textRepository, new CacheResponseProvider())
        {
            var dbValues = _textRepository.GetAll().GetAwaiter().GetResult();
            foreach ( var dbValue in dbValues )
            {
                _cacheProvider.Add(Convert.ToUInt64(dbValue.GuildId), dbValue.Response);
            }
        }
    }
}
