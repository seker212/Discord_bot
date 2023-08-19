namespace DiscordBot.Database.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for guildId/text oriented classes
    /// </summary>
    /// <typeparam name="T">Record from TablesModels</typeparam>
    public interface ITextRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllByGuild(ulong? guildId);
        Task<int> Insert(ulong? guildId, string text);
    }
}
