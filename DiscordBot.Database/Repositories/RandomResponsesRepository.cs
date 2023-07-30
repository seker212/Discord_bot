using DiscordBot.Database.TablesModels;
using SqlKata.Execution;

namespace DiscordBot.Database.Repositories
{
    public interface IRandomResponseRepository
    {
        Task<IEnumerable<RandomResponse>> GetAll();
        Task<IEnumerable<RandomResponse>> GetRandomResponses(ulong? guildId);
        Task<int> Insert(ulong? guildId, string randomResponse);
    }

    public class RandomResponsesRepository : IRandomResponseRepository
    {
        private const string TABLE_NAME = "RandomResponse";
        private readonly QueryFactory _queryFactory;

        public RandomResponsesRepository(QueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<int> Insert(ulong? guildId, string randomResponse)
        {
            var record = await _queryFactory.Query().From(TABLE_NAME)
                .Where(new { GuildId = guildId, Response = randomResponse })
                .FirstOrDefaultAsync<RandomResponse>();

            if (record == null)
            {
                return await _queryFactory.Query().From(TABLE_NAME)
                    .InsertAsync(new { GuildId = guildId, Response = randomResponse });
            }
            else
            {
                return record.Id;
            }
        }

        public async Task<IEnumerable<RandomResponse>> GetAll()
            => await _queryFactory.Query().From(TABLE_NAME).GetAsync<RandomResponse>();

        public async Task<IEnumerable<RandomResponse>> GetRandomResponses(ulong? guildId)
            => await _queryFactory.Query().From(TABLE_NAME)
            .Where(new { GuildId = guildId })
            .GetAsync<RandomResponse>();
    }
}
