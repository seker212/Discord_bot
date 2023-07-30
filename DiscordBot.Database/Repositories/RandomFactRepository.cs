using DiscordBot.Database.TablesModels;
using SqlKata.Execution;

namespace DiscordBot.Database.Repositories
{
    public interface IRandomFactRepository
    {
        Task<IEnumerable<RandomFact>> GetAll();
        Task<IEnumerable<RandomFact>> GetRandomFacts(ulong? guildId);
        Task<int> Insert(ulong? guildId, string randomFact);
    }

    public class RandomFactRepository : IRandomFactRepository
    {
        private const string TABLE_NAME = "RandomFact";
        private readonly QueryFactory _queryFactory;

        public RandomFactRepository(QueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<int> Insert(ulong? guildId, string randomFact)
        {
            var record = await _queryFactory.Query().From(TABLE_NAME)
                .Where(new { GuildId = guildId, Fact = randomFact })
                .FirstOrDefaultAsync<RandomFact>();

            if (record == null)
            {
                return await _queryFactory.Query().From(TABLE_NAME)
                    .InsertAsync(new { GuildId = guildId, Fact = randomFact });
            }
            else
            {
                return record.Id;
            }
        }

        public async Task<IEnumerable<RandomFact>> GetAll()
            => await _queryFactory.Query().From(TABLE_NAME).GetAsync<RandomFact>();

        public async Task<IEnumerable<RandomFact>> GetRandomFacts(ulong? guildId)
            => await _queryFactory.Query().From(TABLE_NAME)
            .Where(new { GuildId = guildId })
            .GetAsync<RandomFact>();
    }
}
