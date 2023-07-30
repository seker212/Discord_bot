namespace DiscordBot.Core.Interfaces
{
    public interface IRandomFactProvider
    {
        public IEnumerable<string>? GetAll(ulong? guildId);
        public void AddAll(ulong? guildId, IEnumerable<string> randomFacts);
        public void Add(ulong? guildId, string randomFact);
    }
}
