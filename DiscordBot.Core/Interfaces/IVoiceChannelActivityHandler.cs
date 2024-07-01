using Discord;
using Discord.WebSocket;

namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Handler for voice channel activities
    /// </summary>
    public interface IVoiceChannelActivityHandler
    {
        /// <summary>
        /// Method for handling user voice channel changes
        /// </summary>
        /// <param name="socketUser">User that joined/changed/left voice channel</param>
        /// <param name="beforeVoiceState">State before action was triggered</param>
        /// <param name="afterVoiceState">State after action was triggered</param>
        /// <returns>Activity task</returns>
        Task ExecuteVoiceUpdate(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState);

        /// <summary>
        /// Method for handling voice channel status/description change
        /// </summary>
        /// <param name="channel">Voice channel which description was changed</param>
        /// <param name="beforeVoiceState">Old description of channel</param>
        /// <param name="afterVoiceState">Newly set description of channel</param>
        /// <returns>Activity task</returns>
        Task ExecuteVoiceChannelStatusUpdate(Cacheable<SocketVoiceChannel, ulong> channel, string oldStatus, string newStatus);
    }
}