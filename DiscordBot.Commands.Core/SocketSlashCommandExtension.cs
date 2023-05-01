using Discord.WebSocket;

namespace DiscordBot.Commands.Core
{
    public static class SocketSlashCommandExtension
    {
        public static object GetRequiredOptionValue(this SocketSlashCommand socketSlashCommand, string optionName)
            => socketSlashCommand.Data.Options.Single(x => x.Name == optionName).Value;

        public static object? GetOptionValue(this SocketSlashCommand socketSlashCommand, string optionName)
            => socketSlashCommand.Data.Options.SingleOrDefault(x => x.Name == optionName)?.Value; 
    }
}
