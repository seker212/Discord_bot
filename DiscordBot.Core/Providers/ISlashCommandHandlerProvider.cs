using Discord.WebSocket;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provides function for handling slash commands
    /// </summary>
    public interface ISlashCommandHandlerProvider
    {
        Task SlashCommandHandlerAsync(SocketSlashCommand commandInput);
    }
}
