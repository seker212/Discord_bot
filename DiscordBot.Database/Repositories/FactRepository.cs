using DiscordBot.Database.Repositories.Interfaces;
using DiscordBot.Database.TablesModels;
using SqlKata.Execution;

namespace DiscordBot.Database.Repositories
{
    /// <summary>
    /// Repository class handling "Facts" table
    /// </summary>
    public interface IFactRepository : ITextRepository<Facts>
    {

    }

    /// <inheritdoc cref="IFactRepository"/>
    public class FactRepository : IFactRepository
    {
        private const string TABLE_NAME = "Facts";
        private readonly QueryFactory _queryFactory;

        public FactRepository(QueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<int> Insert(ulong? guildId, string text)
        {
            var record = await _queryFactory.Query().From(TABLE_NAME)
                .Where(new { GuildId = guildId, Fact = text })
                .FirstOrDefaultAsync<Facts>();

            if (record == null)
            {
                return await _queryFactory.Query().From(TABLE_NAME)
                    .InsertAsync(new { GuildId = guildId, Fact = text });
            }
            else
            {
                return record.Id;
            }
        }

        public async Task<IEnumerable<Facts>> GetAll()
            => await _queryFactory.Query().From(TABLE_NAME).GetAsync<Facts>();

        public async Task<IEnumerable<Facts>> GetAllByGuild(ulong? guildId)
            => await _queryFactory.Query().From(TABLE_NAME)
            .Where(new { GuildId = guildId })
            .GetAsync<Facts>();
    }
}
