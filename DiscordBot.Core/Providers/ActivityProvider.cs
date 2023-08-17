using Discord;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provides data for bot's discord status (activity)
    /// </summary>
    public interface IActivityProvider
    {
        /// <summary>
        /// Discord activity type.
        /// </summary>
        public ActivityType ActivityType { get; }

        /// <summary>
        /// Name for the activity. 
        /// E.g. Name of the game for <see cref="ActivityType.Playing"/> activity type.
        /// </summary>
        public string ActivityName { get; }

        /// <summary>
        /// Url to twitch stream. 
        /// </summary>
        public string? TwitchStreamUrl { get; }
    }

    /// <inheritdoc cref="IActivityProvider"/>
    public class ActivityProvider : IActivityProvider
    {
        /// <summary>
        /// Creates <see cref="ActivityProvider"/> providing given activity.
        /// </summary>
        /// <param name="activityType">Discord activity type.</param>
        /// <param name="activityName">
        /// Name for the activity. 
        /// E.g. Name of the game for <see cref="ActivityType.Playing"/> activity type.
        /// </param>
        /// <param name="twitchStreamUrl">Url to twitch stream.</param>
        /// <exception cref="ArgumentException">
        /// Param <paramref name="twitchStreamUrl"/> was filled, but <paramref name="activityType"/> was not <see cref="ActivityType.Streaming"/>.
        /// </exception>
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
