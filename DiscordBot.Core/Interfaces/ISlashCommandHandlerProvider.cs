using Discord.WebSocket;

namespace DiscordBot.Core.Interfaces
{
    /// <summary>
    /// Provides function for handling slash commands
    /// </summary>
    public interface ISlashCommandHandlerProvider
    {
        /// <summary>
        /// Executes when slash command is received.
        /// </summary>
        /// <param name="commandInput">Executed command</param>
        /// <returns>Task of command execution</returns>
        Task SlashCommandHandlerAsync(SocketSlashCommand commandInput);
    }
}
