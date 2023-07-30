namespace DiscordBot.Core.Interfaces
{
    public interface IRandomResponseProvider
    {
        public List<string> GetAll(ulong? guildId);
        public void AddAll(ulong? guildId, IEnumerable<string> randomResponses);
        public void Add(ulong? guildId, string randomResponse);
    }
}
