using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    /// <summary>
    /// Class for handling adding "random" responses used later by mentioning bot in text channel
    /// </summary>
    [Name("addresponse")]
    [Description("Adds new random response, single from text or all from attached txt file")]
    [Option("text", "Add single random resonse", CommandOptionType.String, false)]
    [Option("file", "Add from file all random resonses (each in new line)", CommandOptionType.Attachment, false)]
    [RequiredPermission(GuildPermission.ManageGuild)]
    public class AddResponseCommand : Command
    {
        private readonly ILogger<AddResponseCommand> _logger;
        private IResponseProvider _responseProvider;

        public AddResponseCommand(ILogger<AddResponseCommand> logger, IResponseProvider responseProvider)
        {
            _logger = logger;
            _responseProvider = responseProvider;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            try
            {
                var guildId = command.GuildId;
                var optionText = command.GetOptionValue<string>("text");
                var optionFile = command.GetOptionValue<Attachment>("file");
                
                if (!string.IsNullOrWhiteSpace(optionText))
                {
                    _responseProvider.Add(guildId, optionText);
                    
                    await command.RespondAsync("Added new random response");
                }
                else if (optionFile is not null)
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(optionFile.Url);

                        var stream = response.Content.ReadAsStream();
                        using (var streamReader = new StreamReader(stream))
                        {
                            var text = streamReader.ReadToEnd().Split("\r\n").AsEnumerable();
                            _responseProvider.AddAll(guildId, text);
                        }

                        await command.RespondAsync("Added new random responses from give file");
                    }                    
                }
                else
                {
                    await command.RespondAsync("Nothing was provided as response to be added");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding new random resonse threw an exception");
                await command.RespondAsync("Error occured when handling command");
            }
        }
    }
}
