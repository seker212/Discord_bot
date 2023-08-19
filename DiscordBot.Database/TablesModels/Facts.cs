namespace DiscordBot.Database.TablesModels
{
    /// <summary>
    /// Definition of Facts table model
    /// </summary>
    public record Facts
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
        public string Fact { get; set; }
    }
}
