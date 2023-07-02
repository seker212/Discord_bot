using DiscordBot.Core.Interfaces;

namespace DiscordBot.Core.Helpers
{
    public class CacheConfigProvider : IConfigProvider
    {
        private readonly Dictionary<ulong, Dictionary<string, string?>> _parametersCache;

        public CacheConfigProvider()
        {
            _parametersCache = new();
        }

        public CacheConfigProvider(Dictionary<ulong, Dictionary<string, string?>> parametersCache)
        {
            _parametersCache = parametersCache;
        }

        public string? GetParameter(ulong? guildId, string parameterName)
        {
            var convertedGuildId = guildId ?? 0;
            if (!_parametersCache.ContainsKey(convertedGuildId))
                return null;
            var guildParams = _parametersCache[convertedGuildId];
            if (!guildParams.ContainsKey(parameterName))
                return null;
            return guildParams[parameterName];
        }

        public void SetParameter(ulong? guildId, string parameterName, string? parameterValue)
        {
            var convertedGuildId = guildId ?? 0;
            if (!_parametersCache.ContainsKey(convertedGuildId))
                _parametersCache.Add(convertedGuildId, new() { { parameterName, parameterValue } });
            else
            {
                var guildParams = _parametersCache[convertedGuildId];
                if (!guildParams.ContainsKey(parameterName))
                    guildParams.Add(parameterName, parameterValue);
                else
                    guildParams[parameterName] = parameterValue;
            }
        }
    }
}
