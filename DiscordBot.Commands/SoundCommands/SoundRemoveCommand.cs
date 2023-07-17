using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Core;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.SoundCommands
{
    [Name("soundremove")]
    [Description("Removes given sound")]
    [Option("name", "Sound name to be removed", CommandOptionType.String)]
    [RequiredPermission(Discord.GuildPermission.ManageGuild)]
    public class SoundRemoveCommand : AbstractSoundCommand
    {
        private readonly ILogger<SoundRemoveCommand> _logger;

        public SoundRemoveCommand(ILogger<SoundRemoveCommand> logger)
        {
            _logger = logger;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();

            try
            {
                var name = command.GetRequiredOptionValue("name");

                var file = new FileInfo(AudioDirectoryPath + name + ".mp3");
                    
                if(file.Exists)
                {
                    file.Delete();
                    await command.ModifyOriginalResponseAsync(m => m.Content = "File " + name + ".mp3 removed");
                }
                else
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "There is no such file to be removed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Removing sound threw an exception");
                await command.ModifyOriginalResponseAsync(m => m.Content = "Error occured when handling command");
            }
        }
    }
}
