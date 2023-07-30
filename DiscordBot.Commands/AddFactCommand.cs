﻿using Discord.WebSocket;
using Discord;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Core;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("addfact")]
    [Description("Adds new random fact, single from text or all from attached txt file")]
    [Option("text", "Add single random fact", CommandOptionType.String, false)]
    [Option("file", "Add from file all random fact (each in new line)", CommandOptionType.Attachment, false)]
    [RequiredPermission(GuildPermission.ManageGuild)]
    public class AddFactCommand : Command
    {
        private readonly ILogger<AddResponseCommand> _logger;
        private IRandomFactProvider _randomFactProvider;

        public AddFactCommand(ILogger<AddResponseCommand> logger, IRandomFactProvider randomFactProvider)
        {
            _logger = logger;
            _randomFactProvider = randomFactProvider;
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
                    _randomFactProvider.Add(guildId, optionText);

                    await command.RespondAsync("Added new random fact");
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
                            _randomFactProvider.AddAll(guildId, text);
                        }

                        await command.RespondAsync("Added new random facts from give file");
                    }
                }
                else
                {
                    await command.RespondAsync("Nothing was provided as fact to be added");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding new random fact threw an exception");
                await command.RespondAsync("Error occured when handling command");
            }
        }
    }
}
