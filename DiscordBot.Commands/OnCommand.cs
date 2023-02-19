using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    [Name("on")]
    [Description("Check if Chi-chan's alive.")]
    public class OnCommand : Command
    {
        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("I'm alive!");
        }
    }
}
