using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Core.Voice;

namespace DiscordBot.Commands
{
    [Name("sound")]
    [Description("Plays sound")]
    public class SoundCommand : Command
    {
        private readonly IAudioClientManager _audioClientManager;

        public SoundCommand(IAudioClientManager audioClientManager)
        {
            _audioClientManager = audioClientManager;
        }

        public override SlashCommandBuilder CustomBuildAction(SlashCommandBuilder slashCommandBuilder) //TODO: Add attribute for basic options
        {
            var soundNameOptionBuilder = new SlashCommandOptionBuilder();
            soundNameOptionBuilder
                .WithName("soundname")
                .WithDescription("sound name")
                .WithType(ApplicationCommandOptionType.String);

            var vcOptionBuilder = new SlashCommandOptionBuilder();
            vcOptionBuilder
                .WithName("channel")
                .WithDescription("voice channel")
                .WithType(ApplicationCommandOptionType.Channel);

            return slashCommandBuilder.AddOptions(vcOptionBuilder, soundNameOptionBuilder);
        }

        public override Task ExecuteAsync(SocketSlashCommand command)
        {
            Task.Run(async () =>
            {
                var audioFile = new FileInfo(Path.Combine(@"..\..\..\..\audio\", command.Data.Options.Single(x => x.Name == "soundname").Value as string));
                if (!audioFile.Exists)
                    throw new FileNotFoundException(audioFile.FullName);
                var targetChannel = command.Data.Options.Single(x => x.Name == "channel").Value as SocketChannel;
                if (targetChannel is SocketVoiceChannel voiceChannel)
                {
                    var stream = new MemoryStream(); //TODO: Change this to PCM Stream
                    var audioClient = await _audioClientManager.JoinChannelAsync(voiceChannel);
                    var player = _audioClientManager.GetAudioPlayer(audioClient);
                    await player.PlayAsync(stream);
                    await _audioClientManager.LeaveChannelAsync(voiceChannel);
                }
            });
            return Task.Run(() => { command.RespondAsync("placeholder"); });
        }
    }
}
