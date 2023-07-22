using Discord.WebSocket;

namespace DiscordBot.Commands.Core
{
    public static class SocketSlashCommandExtension
    {
        public static object? GetOptionValue(this SocketSlashCommand socketSlashCommand, string optionName)
            => socketSlashCommand.Data.Options.SingleOrDefault(x => x.Name == optionName)?.Value;

        public static T? GetOptionValue<T>(this SocketSlashCommand socketSlashCommand, string optionName) where T : class
        {
            var value = socketSlashCommand.GetOptionValue(optionName);
            if (value is null)
                return null;
            if (value is T castedValue)
                return castedValue;
            else
                throw new InvalidCastException($"Unable to cast {value.GetType()} into {typeof(T)} for option {optionName}.");
        }

        public static object GetRequiredOptionValue(this SocketSlashCommand socketSlashCommand, string optionName)
            => socketSlashCommand.GetOptionValue(optionName) ?? throw new ArgumentNullException($"Option {optionName} was null.");

        public static T GetRequiredOptionValue<T>(this SocketSlashCommand socketSlashCommand, string optionName) where T : class
        {
            var value = socketSlashCommand.GetRequiredOptionValue(optionName);
            return value as T ?? throw new InvalidCastException($"Unable to cast {value.GetType()} into {typeof(T)} for option {optionName}.");
        }
    }
}
