using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    /// <summary>
    /// Class for handling help command
    /// </summary>
    [Name("help")]
    [Description("Prints help command")]
    [Option("command", "displays command details", CommandOptionType.String, false)]
    public class HelpCommand : Command 
    {        
        private readonly ILogger<HelpCommand> _logger;
        private Lazy<IEnumerable<ICommand>> _commands;

        public HelpCommand(ILogger<HelpCommand> logger, Lazy<IEnumerable<ICommand>> commands)
        {
            _logger = logger;
            _commands = commands;
        }

        /// <summary>
        /// Method for obtaining command to specific user guild permissions
        /// </summary>
        /// <param name="userPermissions">Discord guild permission</param>
        /// <returns>List of command with command that this user can use</returns>
        private List<ICommand> GetCommands(GuildPermissions userPermissions)
        {
            var commands = _commands.Value.ToList();

            foreach (var command in commands)
            {
                var permission = command.RequiredGuildPermission;

                if (permission != null && !userPermissions.Has(permission.Value))
                {
                    commands.Remove(command);
                }
            }

            return commands;
        }

        /// <summary>
        /// Embed builder for specific command help
        /// </summary>
        /// <param name="userPermissions">Discord guild permission</param>
        /// <param name="command">command that is queried for detailed help</param>
        /// <returns>Embed with all informations about specific command</returns>
        private Embed GetCommandHelp(GuildPermissions userPermissions, string command) 
        {
            var commands = GetCommands(userPermissions);

            command = command.ToLower();
            command = command.Replace("/", "");

            var title = "Help for `/" + command + "`";
            var desription = "";

            var result = commands.Find(x => x.Name == command);

            if(result == null)
            {
                desription = "There is no such command";
            }
            else 
            {
                desription = result.Description + "\r\n";

                string options = "";

                foreach (var option in result.Options)
                {
                    options += "​​​​​​`" + option.Name + "` - " + option.Description;

                    if (option.IsRequired)
                    {
                        options += " (Required) ";
                    }
                    else
                    {
                        options += " (Optional) ";
                    }

                    options += "\r\n";
                }

                if (options.Length > 0)
                {
                    desription += "Options:";
                    desription += options;
                }
            }

            var embed = new EmbedBuilder()
            {
                Title = title,
                Description = desription,
                Color = Color.DarkTeal,
            };

            return embed.Build();
        }

        /// <summary>
        /// Embed builder for all commands help
        /// </summary>
        /// <param name="userPermissions">Discord guild permission</param>
        /// <returns>Embed with all commands with their descriptions</returns>
        private Embed GetAllCommandsHelp(GuildPermissions userPermissions)
        {
            var commands = GetCommands(userPermissions);

            var desciption = "\r\n";

            foreach (var command in commands)
            {
                desciption += "`/" + command.Name + "` - " + command.Description + "\r\n";
            }

            var embed = new EmbedBuilder()
            {
                Title = "Help",
                Description = desciption,
                Color = Color.DarkTeal,
            };

            var footer = new EmbedFooterBuilder()
            {
                Text = "Use `/help <command>` to get more information about command",
            };

            embed.WithFooter(footer);

            return embed.Build();
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();

            try
            {
                var user = command.User as IGuildUser;

                if (user == null)
                {
                    throw new ArgumentException("User in help command was null");
                }

                var option = command.GetOptionValue<string>("command");

                Embed embed;

                if (option != null)
                {
                    embed = GetCommandHelp(user.GuildPermissions, option);
                }
                else
                {
                    embed = GetAllCommandsHelp(user.GuildPermissions);
                }

                await command.ModifyOriginalResponseAsync(m => m.Embed = embed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Printing help threw an exception");
                await command.ModifyOriginalResponseAsync(m => m.Content = "Error occured when handling command");
            }
        }

    }
}
