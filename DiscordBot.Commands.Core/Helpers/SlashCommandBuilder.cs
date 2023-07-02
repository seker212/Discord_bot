using Discord;

namespace DiscordBot.Commands.Core.Helpers
{
    public interface ISlashCommandBuilder
    {
        SlashCommandProperties Build(ICommand command);
    }

    public class SlashCommandBuilder : ISlashCommandBuilder
    {
        private readonly ICommandOptionConverter _commandOptionConverter;

        public SlashCommandBuilder(ICommandOptionConverter commandOptionConverter)
        {
            _commandOptionConverter = commandOptionConverter;
        }

        public SlashCommandProperties Build(ICommand command)
        {
            var builder = new Discord.SlashCommandBuilder()
                .WithName(command.Name)
                .WithDescription(command.Description)
                .WithDefaultMemberPermissions(command.RequiredGuildPermission);
            foreach (var option in command.Options)
            {
                ChannelType? channelType = option.Type switch
                {
                    CommandOptionType.GuildTextChannel => ChannelType.Text,
                    CommandOptionType.GuildVoiceChannel => ChannelType.Voice,
                    _ => null
                };
                builder.AddOption(option.Name, _commandOptionConverter.ConvertOptionType(option.Type), option.Description, option.IsRequired, channelTypes: channelType.HasValue ? new List<ChannelType>() { channelType.Value } : null);
            }
            return command.CustomBuildAction(builder).Build();
        }
    }
}
