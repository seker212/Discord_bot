using Discord.WebSocket;
using Discord;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Core;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;
using DiscordBot.Core.Helpers;

namespace DiscordBot.Commands
{
    [Name("settimezone")]
    [Description("Set timezone for printed logs")]
    [Option("timezone", "Time zone in any correct format", CommandOptionType.String)]
    [RequiredPermission(GuildPermission.ManageGuild)]
    public class SetTimeZoneCommand : Command
    {
        private readonly ILogger<SetTimeZoneCommand> _logger;
        private IConfigProvider _configProvider;
        private ITimezoneHelper _timeZoneHelper;

        public SetTimeZoneCommand(ILogger<SetTimeZoneCommand> logger, IConfigProvider configProvider, ITimezoneHelper timezoneHelper)
        {
            _logger = logger;
            _configProvider = configProvider;
            _timeZoneHelper = timezoneHelper;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            try
            {
                var guildId = command.GuildId!;
                var option = command.Data.Options.Single(x => x.Name == "timezone").Value as string;

                var timezone = _timeZoneHelper.ConvertTimeZoneFromString(option!);

                if (timezone != null)
                {
                    _logger.LogDebug("Set timezone to {ch} in guild {id}", option, guildId);
                    _configProvider.SetParameter(guildId.Value, "TimeZoneValue", timezone.Id);
                    await command.RespondAsync("Timezone set");
                }
                else
                    await command.RespondAsync("Given value should be correct timezone");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Setting timezone threw an exception");
                await command.RespondAsync("Error occured when handling command");
            }
        }
    }
}
