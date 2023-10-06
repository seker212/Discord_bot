using DiscordBot.ActivityLogging.Enums;

namespace DiscordBot.ActivityLogging.Helpers.Models
{
    public record LogActivityContent
    {
        public LogActivityContent(LogActivityType logActivityType, ulong guildId, string? mention, string? voiceAction) : this(logActivityType, guildId, mention, voiceAction, null)
        {

        }

        public LogActivityContent(LogActivityType logActivityType, ulong guildId, string? mention, string? voiceAction, string? textAction)
        {
            LogActivityType = logActivityType;
            GuildId = guildId;
            Mention = mention;
            VoiceAction = voiceAction;
            TextAction = textAction;
        }

        public LogActivityType LogActivityType { get; set; }
        public ulong GuildId { get; set; }
        public string? Mention { get; set; }
        public string? VoiceAction { get; set; }
        public string? TextAction { get; set; }
    }
}
