namespace DiscordBot.Core.Providers
{
    public interface IChannelDataProvider
    {
        ulong? getChannel(ulong guildId);
        void setChannel(ulong guildId, ulong channelId);
    }

    public class ChannelDataProvider : IChannelDataProvider
    {
        private Dictionary<ulong, ulong> channelDict;

        public ChannelDataProvider()
        {
            channelDict = new Dictionary<ulong, ulong>();
        }

        public ulong? getChannel(ulong guildId)
        {
            if(channelDict.ContainsKey(guildId))
            {
                return channelDict[guildId];
            }
            return null;
        }

        public void setChannel(ulong guildId, ulong channelId)
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