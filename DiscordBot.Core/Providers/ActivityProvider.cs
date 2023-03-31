using Discord;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provides data for bot's discord status (activity)
    /// </summary>
    public interface IActivityProvider
    {
        public ActivityType ActivityType { get; }
        public string ActivityName { get; }
        public string? TwitchStreamUrl { get; }
    }

    /// <inheritdoc cref="IActivityProvider"/>
    public class ActivityProvider : IActivityProvider
    {
        public ActivityProvider(ActivityType activityType, string activityName, string? twitchStreamUrl = null)
        {
            if (!string.IsNullOrEmpty(twitchStreamUrl) && activityType != ActivityType.Streaming)
                throw new ArgumentException($"Cannot create activity of other type than {ActivityType.Streaming} with stream url");
            ActivityType = activityType;
            ActivityName = activityName;
            TwitchStreamUrl = twitchStreamUrl;
        }

        public ActivityType ActivityType { get; }

        public string ActivityName { get; }

        public string? TwitchStreamUrl { get; }
    }
}
