using DiscordBot.Commands.Helpers.Models;
using DiscordBot.Core.Voice;
using System.Diagnostics;

namespace DiscordBot.Commands.Helpers
{
    public interface IAudioStreamHelper
    {
        AudioStreamElements CreateAudioStream(FileInfo audioFile);
        AudioStreamElements CreateAudioStream(YoutubeVideoData videoData);
    }

    public class AudioStreamHelper : IAudioStreamHelper
    {
        private const string FFMPEG = "ffmpeg";
        private const string AUDIO_PARAMS = "-ac 2 -f s16le -ar 48000 pipe:1";
        private const string LOG_LEVEL = "-loglevel panic -hide_banner";

        public AudioStreamHelper() { }

        public AudioStreamElements CreateAudioStream(FileInfo audioFile)
        {
            var ffmpegProcess = Process.Start(new ProcessStartInfo
            {
                FileName = FFMPEG,
                Arguments = $"{LOG_LEVEL} -i \"{audioFile.FullName}\" {AUDIO_PARAMS}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            return new AudioStreamElements(ffmpegProcess.StandardOutput.BaseStream, new[] { ffmpegProcess });
        }

        public AudioStreamElements CreateAudioStream(YoutubeVideoData videoData)
        {
            var ffmpegProcess = Process.Start(new ProcessStartInfo
            {
                FileName = FFMPEG,
                Arguments = $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 {LOG_LEVEL} -i \"{videoData.DownloadUrl}\"{AUDIO_PARAMS}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            return new AudioStreamElements(ffmpegProcess.StandardOutput.BaseStream, new[] { ffmpegProcess });
        }
    }
}
