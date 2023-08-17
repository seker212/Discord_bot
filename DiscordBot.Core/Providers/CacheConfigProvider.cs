using DiscordBot.Core.Interfaces;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provides configuration values using runtime cache.
    /// </summary>
    public class CacheConfigProvider : IConfigProvider
    {
        private readonly Dictionary<ulong, Dictionary<string, string?>> _parametersCache;

        /// <summary>
        /// Creates new <see cref="CacheConfigProvider"/> with empty cache.
        /// </summary>
        public CacheConfigProvider()
        {
            _parametersCache = new();
        }

        /// <summary>
        /// Creates new <see cref="CacheConfigProvider"/> with pre-initialized cache.
        /// </summary>
        /// <param name="parametersCache">
        /// Pre-initialized cache with whe first key being guild id, and the second the parameter name. 
        /// </param>
        public CacheConfigProvider(Dictionary<ulong, Dictionary<string, string?>> parametersCache)
        {
            _parametersCache = parametersCache;
        }

        /// <inheritdoc cref="IConfigProvider.GetParameter(ulong?, string)"/>
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

        /// <inheritdoc cref="IConfigProvider.SetParameter(ulong?, string, string?)"/>
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
