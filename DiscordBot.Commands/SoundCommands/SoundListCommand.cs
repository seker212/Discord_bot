using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.SoundCommands
{
    [Name("soundlist")]
    [Description("Prints the list of available sounds")]
    [Option("filter", "For getting only sound starting from given value", CommandOptionType.String, false)]
    public class SoundListCommand : AbstractSoundCommand
    {
        private readonly ILogger<SoundListCommand> _logger;
        private readonly List<char> alphabet = new List<char>{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y', 'Z'};

        public SoundListCommand(ILogger<SoundListCommand> logger)
        {
            _logger = logger;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            command.DeferAsync().Wait();

            try
            {
                var filter = command.GetOptionValue("filter") as string;

                if (filter != null) 
                {
                    await command.ModifyOriginalResponseAsync(m => m.Embed = GetSoundListEmbed(filter));
                }
                else
                {
                    await command.ModifyOriginalResponseAsync(m => m.Embed = GetSoundListEmbed());
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Printing sound list threw an exception");
                await command.ModifyOriginalResponseAsync(m => m.Content = "Error occured when handling command");
            }
        }

        private Embed GetSoundListEmbed(string filter = "")
        {
            var soundNames = GetSoundNames();
            var embed = new EmbedBuilder()
            {
                Title = "Sound list",
                Color = Color.Teal,
            };

            if (filter != string.Empty)
            {
                soundNames = soundNames.Where(x => x.StartsWith(filter));

                embed.WithDescription("List of all sounds with applied filter: " + filter);
            }
            else
            {
                embed.WithDescription("List of all sounds");
            }

            foreach (var letter in alphabet)
            {
                string text = string.Join("\n", soundNames.Where(x => x.ToUpper().First().Equals(letter)));

                if (text != string.Empty)
                {
                    embed.AddField(letter.ToString(), text);
                }
            }

            return embed.Build();
        }
    }
}
