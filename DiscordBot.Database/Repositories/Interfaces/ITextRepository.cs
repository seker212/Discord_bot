namespace DiscordBot.Database.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for guildId/text oriented classes
    /// </summary>
    /// <typeparam name="T">Record from TablesModels</typeparam>
    public interface ITextRepository<T> where T : class
    {
        /// <summary>
        /// Method for obtaining all records for all guilds in form of list
        /// </summary>
        /// <returns>List of all text</returns>
        Task<IEnumerable<T>> GetAll();
        /// <summary>
        /// Method for obtaining all records specific to guild in form of list
        /// </summary>
        /// <param name="guildId">Discord unique GuildId</param>
        /// <returns>List of text specific to given guild</returns>
        Task<IEnumerable<T>> GetAllByGuild(ulong? guildId);
        /// <summary>
        /// Add new text to a given guild
        /// </summary>
        /// <param name="guildId">Discord unique GuildId</param>
        /// <param name="text">Text to be added</param>
        /// <returns>Record uniqe identifier(id)</returns>
        Task<int> Insert(ulong? guildId, string text);
    }
}
