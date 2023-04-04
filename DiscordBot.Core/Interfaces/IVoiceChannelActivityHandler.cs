using Discord.WebSocket;

namespace DiscordBot.Core.Interfaces
{
    public interface IVoiceChannelActivityHandler
    {
        Task Execute(SocketUser socketUser, SocketVoiceState beforeVoiceState, SocketVoiceState afterVoiceState);
    }
}