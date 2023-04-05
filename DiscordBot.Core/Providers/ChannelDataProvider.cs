namespace DiscordBot.Core.Providers
{
    public interface IChannelDataProvider
    {
        ulong? GetChannel(ulong guildId);
        void SetChannel(ulong guildId, ulong channelId);
    }

    public class ChannelDataProvider : IChannelDataProvider
    {
        private readonly Dictionary<ulong, ulong> _channelDict;

        public ChannelDataProvider()
        {
            _channelDict = new Dictionary<ulong, ulong>();
        }

        public ulong? GetChannel(ulong guildId) 
            => _channelDict.ContainsKey(guildId) ? _channelDict[guildId] : null;

        public void SetChannel(ulong guildId, ulong channelId)
        {
            if(_channelDict.ContainsKey(guildId))
                _channelDict[guildId] = channelId;
            else 
                _channelDict.Add(guildId, channelId);
        }
    }
}