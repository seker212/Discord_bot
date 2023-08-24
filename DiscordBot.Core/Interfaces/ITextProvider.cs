namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Provider interface for text data specific to guildId
    /// </summary>
    public interface ITextProvider
    {
        /// <summary>
        /// Method for obtaining all records based on guildId
        /// </summary>
        /// <param name="guildId">Discord guildId to which elements belongs</param>
        /// <returns>Records based on guildId</returns>
        public IEnumerable<string>? GetAll(ulong? guildId);
        
        /// <summary>
        /// Add all elements from given list based on guildId 
        /// </summary>
        /// <param name="guildId">Discord guildId to which element should belong</param>
        /// <param name="textsList">List of elements to be added</param>
        public void AddAll(ulong? guildId, IEnumerable<string> textsList);

        /// <summary>
        /// Add single element based on guildId
        /// </summary>
        /// <param name="guildId">Discord guildId to which element should belong</param>
        /// <param name="text">Single element to be added</param>
        public void Add(ulong? guildId, string text);
    }
}
