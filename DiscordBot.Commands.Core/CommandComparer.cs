using Discord.WebSocket;

namespace DiscordBot.Commands.Core
{
    public class CommandComparer
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
            var sameArgumentsCount = command.Options.Count == socketCommand.Data.Options.Count;
            var sameOptions = command.Options.All(x => socketCommand.Data.Options.Any(y => CommndOptionSyntaxEquals(x, y)));
            return sameName && sameArgumentsCount && sameOptions;
        }

        /// <summary>
        /// Checks if command options syntax is the same (arguments' names, types). Does not check non-functional parameters like descriptions.
        /// </summary>
        /// <param name="commandOption"></param>
        /// <param name="socketCommandOption"></param>
        /// <returns></returns>
        public bool CommndOptionSyntaxEquals(ICommandOption commandOption, SocketSlashCommandDataOption socketCommandOption)
        {
            var t1 = commandOption.Name.Equals(socketCommandOption.Name);
            var t2 = commandOption.Type.Equals(socketCommandOption.Type);
            return t1 && t2;
        }
    }
}
