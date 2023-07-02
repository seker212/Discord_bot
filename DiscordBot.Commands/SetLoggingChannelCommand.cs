using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands
{
    [Name("setloggingchannel")]
    [Description("Set channel for displaying logs")]
    [Option("channel", "text channel", CommandOptionType.GuildTextChannel)]
    [RequiredPermission(GuildPermission.ManageGuild)]
    public class SetLoggingChannelCommand : Command
    {
        private readonly ILogger<SetLoggingChannelCommand> _logger;
        private IConfigProvider _configProvider;

        public SetLoggingChannelCommand(ILogger<SetLoggingChannelCommand> logger, IConfigProvider configProvider)
        {
            _logger = logger;
            _configProvider = configProvider;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            try 
            {
                var guildId = command.GuildId;
                var targetChannel = command.GetRequiredOptionValue("channel") as SocketChannel;

                if(targetChannel is SocketTextChannel textChannel)
                {
                    _logger.LogDebug("Set logger channel to {ch} in guild {id}", textChannel, guildId);
                    _configProvider.SetParameter(guildId.Value, "LoggingChannelId", textChannel.Id.ToString());
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
