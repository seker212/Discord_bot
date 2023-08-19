namespace DiscordBot.Database.TablesModels
{
    /// <summary>
    /// Definition of Responses table definition
    /// </summary>
    public record Responses
    {
        /// <summary>
        /// Unique indentifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Discord unique for each guild (server) indentifier
        /// </summary>
        public string GuildId { get; set; }

        /// <summary>
        /// Fact text
        /// </summary>
        public string Response { get; set; }
    }
}
