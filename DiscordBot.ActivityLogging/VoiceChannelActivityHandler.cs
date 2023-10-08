using Discord.WebSocket;
using DiscordBot.ActivityLogging.Enums;
using DiscordBot.ActivityLogging.Helpers;
using DiscordBot.ActivityLogging.Helpers.Models;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.ActivityLogging
{
    /// <inheritdoc cref="IVoiceChannelActivityHandler"/>
    public class VoiceChannelActivityHandler : IVoiceChannelActivityHandler
    {
        private const LogActivityType activityType = LogActivityType.VoiceChannelActivity;
        private readonly ILogSenderHelper _logSenderHelper;
        private readonly ILogger<VoiceChannelActivityHandler> _logger;

        public VoiceChannelActivityHandler(ILogSenderHelper logSenderHelper, ILogger<VoiceChannelActivityHandler> logger)
        {
            _logSenderHelper = logSenderHelper;
            _logger = logger;
        }

        public Task Execute(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState)
            => Task.Run(async () =>
            {
                var userMention = socketUser.Mention;
                ulong guildId = beforeVoiceState.VoiceChannel == null ? afterVoiceState.VoiceChannel.Guild.Id : beforeVoiceState.VoiceChannel.Guild.Id;
                string voiceAction = "";

                if (afterVoiceState.VoiceChannel == null)
                    voiceAction = "left voice channel " + beforeVoiceState.VoiceChannel.Mention;
                else if(beforeVoiceState.VoiceChannel == null)
                    voiceAction = "joined voice channel " + afterVoiceState.VoiceChannel.Mention;
                else if(afterVoiceState.VoiceChannel != beforeVoiceState.VoiceChannel)
                    voiceAction = "changed voice channel " + beforeVoiceState.VoiceChannel.Mention + " to " + afterVoiceState.VoiceChannel.Mention;
                else if(afterVoiceState.IsDeafened != beforeVoiceState.IsDeafened)
                {
                    if(afterVoiceState.IsDeafened)
                        voiceAction = "fully muted by guild";
                    else
                        voiceAction = "fully unmuted by guild";
                }
                else if(afterVoiceState.IsMuted != beforeVoiceState.IsMuted)
                {
                    if(afterVoiceState.IsMuted)
                        voiceAction = "muted by guild";
                    else 
                        voiceAction = "unmuted by guild";
                }
                else if(afterVoiceState.IsSelfDeafened != beforeVoiceState.IsSelfDeafened)
                {
                    if(afterVoiceState.IsSelfDeafened)
                        voiceAction = "fully muted by himself";
                    else 
                        voiceAction = "fully unmuted by himself";
                }
                else if(afterVoiceState.IsSelfMuted != beforeVoiceState.IsSelfMuted)
                {
                    if(afterVoiceState.IsSelfMuted)
                        voiceAction = "muted by himself";
                    else 
                        voiceAction = "unmuted by himself";
                }
                else if(afterVoiceState.IsStreaming != beforeVoiceState.IsStreaming)
                {
                    if(afterVoiceState.IsStreaming)
                        voiceAction = "started streaming";
                    else 
                        voiceAction = "stopped streaming";
                }
                else if(afterVoiceState.IsVideoing != beforeVoiceState.IsVideoing)
                {
                    if(afterVoiceState.IsVideoing)
                        voiceAction = "shows himself in video call";
                    else 
                        voiceAction = "hides himself in video call";
                }

                if(!string.IsNullOrEmpty(voiceAction))
                {
                    await _logSenderHelper.SendLogsToChannel(new LogActivityContent(activityType, guildId, userMention, voiceAction));
                }
            });
    }
}