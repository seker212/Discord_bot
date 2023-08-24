using DiscordBot.Commands.Core;

namespace DiscordBot.Commands.SoundCommands
{
    public abstract class AbstractSoundCommand : Command
    {
        protected string AudioDirectoryPath { get; } = Environment.GetEnvironmentVariable("AUDIO_PATH")!;

        public IEnumerable<string> GetSoundNames()
        {
            return new DirectoryInfo(AudioDirectoryPath)
                .EnumerateFiles().Where(x => x.Name.EndsWith(".mp3")).Select(x => x.Name.Replace(".mp3", ""));
        }
    }
}
