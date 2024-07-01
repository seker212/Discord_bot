using Discord.WebSocket;
using DiscordBot.ActivityLogging.Enums;
using DiscordBot.ActivityLogging.Helpers;
using DiscordBot.ActivityLogging.Helpers.Models;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.ActivityLogging
{
    /// <summary>
    /// Class for handling logging custom voice chat presence.
    /// </summary>
    public class UserCustomPresenceHandler : IUserPresenceActivityHandler
    {
        private const LogActivityType activityType = LogActivityType.UserCustomStatusChange;
        private readonly ILogSenderHelper _logSenderHelper;
        private readonly ILogger<UserCustomPresenceHandler> _logger;

        public UserCustomPresenceHandler(ILogSenderHelper logSenderHelper, ILogger<UserCustomPresenceHandler> logger)
        {
            _logSenderHelper = logSenderHelper;
            _logger = logger;
        }

        public Task Execute(SocketUser socketUser, SocketPresence oldPresence, SocketPresence newPresence)
            => Task.Run(async () =>
            {
                if (socketUser.IsBot)
                {
                    return;
                }

                if (oldPresence == null || newPresence == null)
                {
                    return;
                }

                var guilds = socketUser.MutualGuilds;

                var oldActivity = oldPresence.Activities.SingleOrDefault(activity => activity.Name == UserPresenceNames.voiceStatusName);
                var newActivity = newPresence.Activities.SingleOrDefault(activity => activity.Name == UserPresenceNames.voiceStatusName);

                var mention = socketUser.Mention;

                if (newActivity == null)
                {
                    //User removed presence, no need for other log.
                    return;
                }

                if(oldActivity != null && oldActivity.Details.Equals(newActivity.Details))
                {
                    //There was no custom status change
                    return;
                }

                var action = "";

                if (oldActivity == null)
                {
                    action = " set status to `" + newActivity.Details + "'";
                }
                else
                {
                    action = " changed status from `" + oldActivity.Details + "` to `" + newActivity.Details + "`";
                }

                if (!string.IsNullOrEmpty(action))
                {
                    foreach (var guild in guilds)
                    {
                        await _logSenderHelper.SendLogsToChannel(new LogActivityContent(activityType, guild.Id, mention, action));
                    }
                }
            });
    }
}
