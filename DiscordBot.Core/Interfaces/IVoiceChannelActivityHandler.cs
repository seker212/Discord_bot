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
        Task Execute(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState);
    }
}