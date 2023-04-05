using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Providers;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("setloggingchannel")]
    [Description("Set channel for displaying logs")]
    [Option("channel", "text channel", ApplicationCommandOptionType.Channel)]
    public class SetLoggingChannelCommand : Command
    {
        private readonly ILogger<SetLoggingChannelCommand> _logger;
        private IChannelDataProvider _channelDataProvider;

        public SetLoggingChannelCommand(ILogger<SetLoggingChannelCommand> logger, IChannelDataProvider channelDataProvider)
        {
            _logger = logger;
            _channelDataProvider = channelDataProvider;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            try 
            {
                var guildId = command.GuildId;
                var targetChannel = command.Data.Options.Single(x => x.Name == "channel").Value as SocketChannel;

                if(targetChannel is SocketTextChannel textChannel)
                {
                    _logger.LogDebug("Set logger channel to {ch} in guild {id}", textChannel, guildId);
                    _channelDataProvider.SetChannel(guildId.Value, textChannel.Id);
                    await command.RespondAsync("Channel set");
                } 
                else
                    await command.RespondAsync("Given channel should be a text channel");
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Setting log channel threw an exception");
                await command.RespondAsync("Error occured when handling command");
            }
        }
    }
}
