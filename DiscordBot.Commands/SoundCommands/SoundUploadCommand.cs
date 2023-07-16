using Discord.WebSocket;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Core;
using Microsoft.Extensions.Logging;
using Discord;

namespace DiscordBot.Commands.SoundCommands
{
    [Name("soundupload")]
    [Description("For uploading new sounds for bot to play")]
    [Option("file", "File in mp3 fromat", CommandOptionType.Attachment)]
    public class SoundUploadCommand : AbstractSoundCommand
    {
        private readonly ILogger<SoundUploadCommand> _logger;

        public SoundUploadCommand(ILogger<SoundUploadCommand> logger)
        {
            _logger = logger;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            command.DeferAsync().Wait();

            try
            {
                var attachment = command.GetRequiredOptionValue("file") as Attachment;
                
                if (attachment.Filename.EndsWith(".mp3")) 
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(attachment.Url);
                        var fileMode = FileMode.CreateNew;
                        var message = "File " + attachment.Filename + " uploaded";

                        if (GetSoundNames().Contains(attachment.Filename))
                        {
                            fileMode = FileMode.Truncate;
                            message = "File " + attachment.Filename + " was replaced";
                        }

                        using (var file = new FileStream(AudioDirectoryPath + "/" + attachment.Filename, fileMode))
                        {
                            await response.Content.CopyToAsync(file);
                            await command.ModifyOriginalResponseAsync(m => {
                                m.Content = message;
                                m.Attachments = new List<FileAttachment> { new FileAttachment(file, attachment.Filename) };
                                });
                        }
                    }
                }
                else
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "Invalid file extension, should be .mp3");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uploading file threw an exception");
                await command.ModifyOriginalResponseAsync(m => m.Content = "Error occured when handling command");
            }
        }
    }
}
