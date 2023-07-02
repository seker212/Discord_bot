using DiscordBot.Core.Providers;
using DiscordBot.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Database
{
    public class LoggingChannelDataProviderDecorator : ILoggingChannelDataProvider
    {
        private readonly ILoggingChannelDataProvider _baseProvider;
        private readonly IConfigRepository _configRepository;
        private const string PARAMETER_NAME = "LoggingChannelId";

        public LoggingChannelDataProviderDecorator(ILoggingChannelDataProvider baseProvider, IConfigRepository configRepository)
        {
            _baseProvider = baseProvider;
            _configRepository = configRepository;

            var entries = _configRepository.GetAllWithKey(PARAMETER_NAME).GetAwaiter().GetResult();
            foreach ( var entry in entries.Where(x => x.ParameterValue is not null) )
                _baseProvider.SetChannel(Convert.ToUInt64(entry.GuildId), Convert.ToUInt64(entry.ParameterValue));
        }

        public ulong? GetChannel(ulong guildId)
            => _baseProvider.GetChannel(guildId);

        public void SetChannel(ulong guildId, ulong channelId)
        {
            _configRepository.InsertOrUpdate(guildId, PARAMETER_NAME, channelId.ToString());
            _baseProvider.SetChannel(guildId, channelId);
        }
    }
}
