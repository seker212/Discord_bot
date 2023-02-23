using Discord.Audio;

namespace DiscordBot.Core.Voice
{
    public enum AudioPlayingStatus
    {
        /// <summary>
        /// Audio is not playing. Anything to be played will start from the beginning.
        /// </summary>
        NotPlaying,

        /// <summary>
        /// Audio is currently playing.
        /// </summary>
        Playing,

        /// <summary>
        /// Audio is paused.
        /// </summary>
        Paused
    }

    public interface IAudioPlayer : IDisposable
    {
        AudioPlayingStatus Status { get; }

        void Pause();
        Task PlayAsync(Stream audioStream);
        void Resume();
        Task StopAsync();
    }

    public sealed class AudioPlayer : IAudioPlayer
    {
        private readonly IAudioClient _audioClient;
        private readonly int _bufferSize;
        private byte[] _buffer;
        private Stream _sourceDataStream;
        private Stream _discordAudioStream;
        private Task? _playingTask;
        private bool _stop;

        public AudioPlayingStatus Status { get; private set; }

        public AudioPlayer(IAudioClient audioClient, int bufferSize)
        {
            _audioClient = audioClient;
            _bufferSize = bufferSize;
            Status = AudioPlayingStatus.NotPlaying;
            _buffer = new byte[_bufferSize];
            _stop = false;
            _playingTask = null;
        }

        public Task PlayAsync(Stream audioStream) //TODO: Add lock?
        {
            //TODO: Check if client is connected
            if (Status == AudioPlayingStatus.Playing || Status == AudioPlayingStatus.Paused)
                throw new InvalidOperationException("Cannot start playing when the player is already playing something.");
            if (_playingTask is not null && !_playingTask.IsCompleted)
                _playingTask.Wait();
            _playingTask = Task.Run(() =>
            {
                _sourceDataStream = audioStream;
                _discordAudioStream = _audioClient.CreatePCMStream(AudioApplication.Mixed);
                Status = AudioPlayingStatus.Playing;
                Play();
            });
            return _playingTask;
        }

        public Task StopAsync()
        {
            return Task.Run(() =>
            {
                _stop = true;
                _playingTask.Wait();
                _stop = false;
                _buffer = new byte[_bufferSize];
                Status = AudioPlayingStatus.NotPlaying;
            });
        }

        public void Pause()
        {
            //TODO: Check if client is connected
            if (Status == AudioPlayingStatus.Playing)
                Status = AudioPlayingStatus.Paused;
        }

        public void Resume() //TODO: Add lock?
        {
            //TODO: Check if client is connected
            if (Status == AudioPlayingStatus.Paused)
                Play();
        }

        private void Play()
        {
            int read;
            Status = AudioPlayingStatus.Playing;
            while (Status == AudioPlayingStatus.Playing && !_stop && (read = _sourceDataStream.Read(_buffer, 0, _buffer.Length)) > 0)
                _discordAudioStream.Write(_buffer, 0, read);
            if (_stop)
            {
                _sourceDataStream.Flush();
                _discordAudioStream.Flush();
                _sourceDataStream.Dispose();
                _discordAudioStream.Dispose();
            }
        }

        public void Dispose()
        {
            _audioClient.Dispose();
            _discordAudioStream.Dispose();
            _sourceDataStream.Dispose();
        }
    }
}
