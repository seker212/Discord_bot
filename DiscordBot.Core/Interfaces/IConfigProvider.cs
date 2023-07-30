namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Provides methods to get and save configuration values.
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// Gets parameter value.
        /// </summary>
        /// <param name="guildId">Guild Id, to which the patameter value is assigned. Null for global parameters.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <returns>Parameter value as string. Null if parameter was not set.</returns>
        public string? GetParameter(ulong? guildId, string parameterName);

        /// <summary>
        /// Sets parameter value. If the parameter already has a value it will be swaped to value provided.
        /// </summary>
        /// <param name="guildId">Guild Id, to which the patameter value is assigned. Null for global parameters.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public void SetParameter(ulong? guildId, string parameterName, string? value);
    }
}
