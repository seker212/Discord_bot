using DiscordBot.Database.Repositories.Interfaces;
using DiscordBot.Database.TablesModels;
using SqlKata.Execution;

namespace DiscordBot.Database.Repositories
{
    /// <summary>
    /// Repository class handling "Responses" table
    /// </summary>
    public interface IResponseRepository : ITextRepository<Responses>
    {

    }

    /// <inheritdoc cref="IResponseRepository"/>
    public class ResponseRepository : IResponseRepository
    {
        private const string TABLE_NAME = "Responses";
        private readonly QueryFactory _queryFactory;

        public ResponseRepository(QueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<int> Insert(ulong? guildId, string text)
        {
            var record = await _queryFactory.Query().From(TABLE_NAME)
                .Where(new { GuildId = guildId, Response = text })
                .FirstOrDefaultAsync<Responses>();

            if (record == null)
            {
                return await _queryFactory.Query().From(TABLE_NAME)
                    .InsertAsync(new { GuildId = guildId, Response = text });
            }
            else
            {
                return record.Id;
            }
        }

        public async Task<IEnumerable<Responses>> GetAll()
            => await _queryFactory.Query().From(TABLE_NAME).GetAsync<Responses>();

        public async Task<IEnumerable<Responses>> GetAllByGuild(ulong? guildId)
            => await _queryFactory.Query().From(TABLE_NAME)
            .Where(new { GuildId = guildId })
            .GetAsync<Responses>();
    }
}
