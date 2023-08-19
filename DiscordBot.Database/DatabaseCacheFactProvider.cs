using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using DiscordBot.Database.Repositories;
using DiscordBot.Database.TablesModels;

namespace DiscordBot.Database
{
    /// <summary>
    /// Database cache provide specific for Facts
    /// </summary>
    public class DatabaseCacheFactProvider : AbstractDatabaseCacheTextProvider<Facts>, IFactProvider
    {
        public DatabaseCacheFactProvider(IFactRepository textRepository) : base(textRepository, new CacheResponseProvider())
        {
            var dbValues = _textRepository.GetAll().GetAwaiter().GetResult();
            foreach (var dbValue in dbValues)
            {
                _cacheProvider.Add(Convert.ToUInt64(dbValue.GuildId), dbValue.Fact);
            }
        }
    }
}
