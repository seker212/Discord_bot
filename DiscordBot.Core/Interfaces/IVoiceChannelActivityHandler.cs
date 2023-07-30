using Discord.WebSocket;

namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Exposes method to execute when any change on voice channel happens.
    /// </summary>
    public interface IVoiceChannelActivityHandler
    {
        /// <summary>
        /// Is executed when any change on voice channel happens.
        /// </summary>
        /// <param name="socketUser">User whose status has changed</param>
        /// <param name="beforeVoiceState">State of voice channel from before the change.</param>
        /// <param name="afterVoiceState">State of voice channel after the change.</param>
        /// <returns></returns>
        Task Execute(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState);
    }
}