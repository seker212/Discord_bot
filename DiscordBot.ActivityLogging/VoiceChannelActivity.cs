using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Interfaces;
using DiscordBot.Core.Providers;
using Microsoft.Extensions.Logging;

namespace DiscordBot.ActivityLogging
{
    public class VoiceChannelActivityHandler : IVoiceChannelActivityHandler
    {
        private readonly ILogger<VoiceChannelActivityHandler> _logger;
        private readonly IChannelDataProvider _channelDataProvider;
        private readonly IDiscordClient _client;

        public VoiceChannelActivityHandler(ILogger<VoiceChannelActivityHandler> logger, IChannelDataProvider channelDataProvider, IDiscordClient client)
        {
            _logger = logger;
            _channelDataProvider = channelDataProvider;
            _client = client;
        }

        private async Task<SocketTextChannel> GetTextChannel(SocketVoiceChannel beforeVoiceChannel, SocketVoiceChannel afterVoiceChannel)
        {
            ulong guildId = beforeVoiceChannel == null ? afterVoiceChannel.Guild.Id : beforeVoiceChannel.Guild.Id;
            var textChannelId = _channelDataProvider.GetChannel(guildId);
            return textChannelId.HasValue ? await _client.GetChannelAsync(textChannelId.Value) as SocketTextChannel : null;
        }

        private async Task SendLogsToChannel(String mention, SocketTextChannel textChannel, String action)
        {
            //TODO datetime and timezone handling, issue #141

            var embed = new EmbedBuilder()
            {
                Title = "Voice status change",
                Description = "<datetime> " + mention + " " +  action,
            };

            embed.WithCurrentTimestamp();
            embed.WithColor(Color.DarkGrey);

            await textChannel.SendMessageAsync(embed: embed.Build());
        }

        public Task Execute(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState)
            => Task.Run(async () =>
            {
                var userMention = socketUser.Mention;
                var textChannel = await GetTextChannel(beforeVoiceState.VoiceChannel, afterVoiceState.VoiceChannel);

                if(textChannel == null)
                    return;

                if(afterVoiceState.VoiceChannel == null)
                    await SendLogsToChannel(userMention, textChannel, "left voice channel " + beforeVoiceState.VoiceChannel.Mention);
                else if(beforeVoiceState.VoiceChannel == null)
                    await SendLogsToChannel(userMention, textChannel, "joined voice channel " + afterVoiceState.VoiceChannel.Mention);
                else if(afterVoiceState.VoiceChannel != beforeVoiceState.VoiceChannel)
                    await SendLogsToChannel(userMention, textChannel, "changed voice channel " + beforeVoiceState.VoiceChannel.Mention + " to " + afterVoiceState.VoiceChannel.Mention);
                else if(afterVoiceState.IsDeafened != beforeVoiceState.IsDeafened)
                {
                    if(afterVoiceState.IsDeafened)
                        await SendLogsToChannel(userMention, textChannel, "fully muted by guild");
                    else
                        await SendLogsToChannel(userMention, textChannel, "fully unmuted by guild");
                }
                else if(afterVoiceState.IsMuted != beforeVoiceState.IsMuted)
                {
                    if(afterVoiceState.IsMuted)
                        await SendLogsToChannel(userMention, textChannel, "muted by guild");
                    else 
                        await SendLogsToChannel(userMention, textChannel, "unmuted by guild");
                }
                else if(afterVoiceState.IsSelfDeafened != beforeVoiceState.IsSelfDeafened)
                {
                    if(afterVoiceState.IsSelfDeafened)
                        await SendLogsToChannel(userMention, textChannel, "fully muted by himself");
                    else 
                        await SendLogsToChannel(userMention, textChannel, "fully unmuted by himself");
                }
                else if(afterVoiceState.IsSelfMuted != beforeVoiceState.IsSelfMuted)
                {
                    if(afterVoiceState.IsSelfMuted)
                        await SendLogsToChannel(userMention, textChannel, "muted by himself");
                    else 
                        await SendLogsToChannel(userMention, textChannel, "unmuted by himself");
                }
                else if(afterVoiceState.IsStreaming != beforeVoiceState.IsStreaming)
                {
                    if(afterVoiceState.IsStreaming)
                        await SendLogsToChannel(userMention, textChannel, "started streaming");
                    else 
                        await SendLogsToChannel(userMention, textChannel, "stopped streaming");
                }
                else if(afterVoiceState.IsVideoing != beforeVoiceState.IsVideoing)
                {
                    if(afterVoiceState.IsVideoing)
                        await SendLogsToChannel(userMention, textChannel, "shows himself in video call");
                    else 
                        await SendLogsToChannel(userMention, textChannel, "hides himself in video call");
                }
            });
    }
}