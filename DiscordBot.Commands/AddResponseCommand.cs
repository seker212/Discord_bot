﻿using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("addresponse")]
    [Description("Adds new random response, single from text or all from attached txt file")]
    [Option("text", "Add single random resonse", CommandOptionType.String, false)]
    [Option("file", "Add from file all random resonses (each in new line)", CommandOptionType.Attachment, false)]
    [RequiredPermission(GuildPermission.ManageGuild)]
    public class AddResponseCommand : Command
    {
        private readonly ILogger<AddResponseCommand> _logger;
        private IRandomResponseProvider _randomResponseProvider;

        public AddResponseCommand(ILogger<AddResponseCommand> logger, IRandomResponseProvider randomResponseProvider)
        {
            _logger = logger;
            _randomResponseProvider = randomResponseProvider;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            try
            {
                var guildId = command.GuildId;
                var optionText = command.GetOptionValue("text") as string;
                var optionFile = command.GetOptionValue("file") as Attachment;
                
                if (string.IsNullOrWhiteSpace(optionText))
                {
                    _randomResponseProvider.Add(guildId, optionText);
                    
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
                            _randomResponseProvider.AddAll(guildId, text);
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
