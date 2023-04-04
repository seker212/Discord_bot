namespace DiscordBot.Core.Providers
{
    public interface IChannelDataProvider
    {
        ulong? GetChannel(ulong guildId);
        void SetChannel(ulong guildId, ulong channelId);
    }

    public class ChannelDataProvider : IChannelDataProvider
    {
        private Dictionary<ulong, ulong> channelDict;

        public ChannelDataProvider()
        {
            channelDict = new Dictionary<ulong, ulong>();
        }

        public ulong? GetChannel(ulong guildId) 
            => channelDict.ContainsKey(guildId) ? channelDict[guildId] : null;

        public void SetChannel(ulong guildId, ulong channelId)
        {
            if(channelDict.ContainsKey(guildId))
            {
                channelDict[guildId] = channelId;
            }
            else 
            {
                channelDict.Add(guildId, channelId);
            }
        }
    }
}