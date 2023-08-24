using Discord.WebSocket;

namespace DiscordBot.Commands.Core
{
    /// <summary>
    /// Helper for comparing commands
    /// </summary>
    public interface ICommandComparer
    {
        bool CommandEquals(ICommand command, SocketApplicationCommand socketCommand);
        bool CommandOptionEquals(ICommandOption commandOption, SocketApplicationCommandOption socketCommandOption);
        bool CommandSyntaxEquals(ICommand command, SocketSlashCommand socketCommand);
        bool CommandOptionSyntaxEquals(ICommandOption commandOption, SocketSlashCommandDataOption socketCommandOption);
    }

    public class CommandComparer : ICommandComparer
    {
        /// <summary>
        /// Checks if commands syntax is the same (name, arguments types). Does not check non-functional parameters like descriptions.
        /// </summary>
        /// <param name="command">First command.</param>
        /// <param name="socketCommand">Second command.</param>
        /// <returns>True if all syntax fields are the same for both commands.</returns>
        public bool CommandSyntaxEquals(ICommand command, SocketSlashCommand socketCommand)
        {
            var sameName = command.Name.Equals(socketCommand.CommandName);
            var sameOptions = command.Options.Where(x => x.IsRequired).All(x => socketCommand.Data.Options.Any(y => CommandOptionSyntaxEquals(x, y)));
            return sameName && sameOptions;
        }

        /// <summary>
        /// Checks if command options syntax is the same (arguments' names, types). Does not check non-functional parameters like descriptions.
        /// </summary>
        /// <param name="commandOption">First command option</param>
        /// <param name="socketCommandOption">Second command option</param>
        /// <returns>True if all fields have the same values.</returns>
        public bool CommandOptionSyntaxEquals(ICommandOption commandOption, SocketSlashCommandDataOption socketCommandOption)
        {
            var sameName = commandOption.Name.Equals(socketCommandOption.Name);
            //TODO: Type comparing
            return sameName;
        }

        /// <summary>
        /// Checks if commands fields have the same values.
        /// </summary>
        /// <param name="command">First command</param>
        /// <param name="socketCommand">Second command</param>
        /// <returns>True if all fields have the same values.</returns>
        public bool CommandEquals(ICommand command, SocketApplicationCommand socketCommand)
        {
            var sameName = command.Name.Equals(socketCommand.Name);
            var sameDescription = command.Description.Equals(socketCommand.Description);
            var sameOptions = command.Options.All(x => socketCommand.Options.Any(y => CommandOptionEquals(x, y)));
            return sameName && sameOptions && sameDescription;
        }

        /// <summary>
        /// Checks if command option's fields have the same values.
        /// </summary>
        /// <param name="commandOption">First command option</param>
        /// <param name="socketCommandOption">Second command option</param>
        /// <returns>True if all fields have the same values.</returns>
        public bool CommandOptionEquals(ICommandOption commandOption, SocketApplicationCommandOption socketCommandOption)
        {
            var sameName = commandOption.Name.Equals(socketCommandOption.Name);
            //TODO: Type comparing
            var sameDescription = commandOption.Description.Equals(socketCommandOption.Description);
            var sameRequirement = commandOption.IsRequired == socketCommandOption.IsRequired;
            return sameName && sameDescription && sameRequirement;
        }
    }
}
