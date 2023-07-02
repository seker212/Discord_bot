namespace DiscordBot.Core.Interfaces
{
    public interface IConfigProvider
    {
        public string? GetParameter(ulong? guildId, string parameterName);
        public void SetParameter(ulong? guildId, string parameterName, string? value);
    }
}
