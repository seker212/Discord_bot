using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.Core.Helpers
{
    public interface ICommandOptionConverter
    {
        ApplicationCommandOptionType ConvertOptionType(CommandOptionType optionType);
    }

    public class CommandOptionConverter : ICommandOptionConverter
    {
        private readonly List<(CommandOptionType CustomType, ApplicationCommandOptionType DiscordType, bool IsDefault)> _types = new()
        {
            (CommandOptionType.String, ApplicationCommandOptionType.String, true),
            (CommandOptionType.Integer, ApplicationCommandOptionType.Integer, true),
            (CommandOptionType.Boolean, ApplicationCommandOptionType.Boolean, true),
            (CommandOptionType.GuildChannel, ApplicationCommandOptionType.Channel, true),
            (CommandOptionType.GuildTextChannel, ApplicationCommandOptionType.Channel, false),
            (CommandOptionType.GuildVoiceChannel, ApplicationCommandOptionType.Channel, false),
            (CommandOptionType.User, ApplicationCommandOptionType.User, true),
            (CommandOptionType.Role, ApplicationCommandOptionType.Role, true),
            (CommandOptionType.UserOrRole, ApplicationCommandOptionType.Mentionable, true),
            (CommandOptionType.Number, ApplicationCommandOptionType.Number, true),
            (CommandOptionType.Attachment, ApplicationCommandOptionType.Attachment, true)
        };


        public ApplicationCommandOptionType ConvertOptionType(CommandOptionType optionType)
        {
            if (_types.Any(x => x.CustomType == optionType))
                return _types.Single(x => x.CustomType == optionType).DiscordType;
            throw new NotSupportedException($"Unable to convert {optionType} to {typeof(ApplicationCommandOptionType)}");
        }
    }
}
