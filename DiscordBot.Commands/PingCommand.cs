using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;

namespace DiscordBot.Commands
{
    [Name("ping")]
    [Description("Check bot ping to gateway.")]
    public class PingCommand : Command
    {
        private DiscordSocketClient _client;

        public PingCommand(DiscordSocketClient client)
        {
            _client = client;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            var ping = _client.Latency;

            await command.RespondAsync("Pong " + ping + "ms");
        }
    }
}
