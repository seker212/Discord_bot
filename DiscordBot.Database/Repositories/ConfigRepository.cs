using DiscordBot.Database.TablesModels;
using SqlKata;
using SqlKata.Execution;

namespace DiscordBot.Database.Repositories
{
    public interface IConfigRepository
    {
        Task<Config> Get(ulong? guildId, string key);
        Task<IEnumerable<Config>> GetAll();
        Task<int> InsertOrUpdate(ulong? guildId, string key, string value);
    }

    public class ConfigRepository : IConfigRepository
    {
        private const string TABLE_NAME = "Config";
        private readonly QueryFactory _queryFactory;

        public ConfigRepository(QueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<int> InsertOrUpdate(ulong? guildId, string key, string value)
        {
            var record = await BuildGetQuery(guildId, key).FirstOrDefaultAsync<Config>();
            if (record == null)
                return await _queryFactory.Query().From(TABLE_NAME).InsertAsync(new { GuildId = guildId, ParameterName = key, ParameterValue = value });
            else
                return await _queryFactory.Query().From(TABLE_NAME).Where(new { record.Id }).UpdateAsync(new { ParameterValue = value });
        }

        public async Task<IEnumerable<Config>> GetAll()
            => await _queryFactory.Query().From(TABLE_NAME).GetAsync<Config>();

        public async Task<Config> Get(ulong? guildId, string key)
            => await BuildGetQuery(guildId, key).FirstOrDefaultAsync<Config>();

        private Query BuildGetQuery(ulong? guildId, string key)
            => _queryFactory.Query().From(TABLE_NAME).Where(new { GuildId = guildId, ParameterName = key });
    }
}
