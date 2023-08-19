using DiscordBot.Core.Interfaces;

using DiscordBot.Database.Repositories.Interfaces;

namespace DiscordBot.Database
{
    /// <summary>
    /// Abstract database cache provider for guildId/text oriented common methods
    /// </summary>
    /// <typeparam name="T">Record from TableModels</typeparam>
    public abstract class AbstractDatabaseCacheTextProvider<T> : ITextProvider where T : class 
    {
        protected readonly ITextProvider _cacheProvider;
        protected readonly ITextRepository<T> _textRepository;

        public AbstractDatabaseCacheTextProvider(ITextRepository<T> textRepository, ITextProvider cacheProvider)
        {
            _textRepository = textRepository;
            _cacheProvider = cacheProvider;
        }

        public void Add(ulong? guildId, string text)
        {
            _textRepository.Insert(guildId, text);
            _cacheProvider.Add(guildId, text);
        }

        public void AddAll(ulong? guildId, IEnumerable<string> textsList)
        {
            foreach (var text in textsList)
            {
                Add(guildId, text);
            }
        }

        public IEnumerable<string>? GetAll(ulong? guildId)
            => _cacheProvider.GetAll(guildId);
    }
}
