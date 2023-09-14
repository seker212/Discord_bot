using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("help")]
    [Description("Prints help command")]
    public class HelpCommand : Command 
    {        
        private readonly ILogger<HelpCommand> _logger;

        public HelpCommand(ILogger<HelpCommand> logger)
        {
            _logger = logger;
        }

        private Embed BuildEmbed(GuildPermissions userPermissions)
        {
            var _commands = CommandListHelper.Commands.ToList();

            var embed = new EmbedBuilder()
            {
                Title = "Help",
                Description = "This is help about bot functionality",
                Color = Color.DarkTeal,
            };

            foreach(var command in _commands)
            {
                var permission = command.RequiredGuildPermission;

                if(permission != null && !userPermissions.Has(permission.Value))
                {
                    continue;
                }

                string options = "";

                foreach(var option in command.Options)
                {
                    options += "`" + option.Name + "` - " + option.Description + "\r\n";
                }

                string description = command.Description;

                if(options.Length > 0)
                {
                    description += "\r\n";
                    description += "Options: \r\n";
                    description += options;
                }

                embed.AddField(command.Name, description);
            }

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
                    _logger.LogError("User in help command was null");

                    return;
                }

                var embed = BuildEmbed(user.GuildPermissions);
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
