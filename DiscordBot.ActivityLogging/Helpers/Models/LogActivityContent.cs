using DiscordBot.ActivityLogging.Enums;

namespace DiscordBot.ActivityLogging.Helpers.Models
{
    /// <summary>
    /// Class for transfering information about activity logs.
    /// </summary>
    public record LogActivityContent
    {
        public LogActivityContent(LogActivityType logActivityType, ulong guildId, string? mention, string? voiceAction) : this(logActivityType, guildId, mention, voiceAction, null)
        {

        }

        /// <summary>
        /// Default all parameters constructor.
        /// </summary>
        /// <param name="logActivityType">Type of activity</param>
        /// <param name="guildId">Discord guild id associated with action</param>
        /// <param name="mention">Mention of user associated with action</param>
        /// <param name="voiceAction">Text information about voice action that will be logged</param>
        /// <param name="textAction">Text information about text/message action that will be logged</param>
        public LogActivityContent(LogActivityType logActivityType, ulong guildId, string? mention, string? voiceAction, string? textAction)
        {
            LogActivityType = logActivityType;
            GuildId = guildId;
            Mention = mention;
            VoiceAction = voiceAction;
            TextAction = textAction;
        }

        public LogActivityType LogActivityType { get; }
        public ulong GuildId { get; }
        public string? Mention { get; }
        public string? VoiceAction { get; }
        public string? TextAction { get; }
    }
}
