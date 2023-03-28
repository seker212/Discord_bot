using Discord;
using Discord.Audio;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace DiscordBot.Core.Voice
{
    public interface IAudioClientManager : IDisposable
    {
        /// <summary>
        /// Gets or creates audio player for given client.
        /// </summary>
        /// <param name="audioClient">Audio client</param>
        /// <returns>Audio player</returns>
        IAudioPlayer GetAudioPlayer(IAudioClient audioClient);

        /// <summary>
        /// Returns guild's voice channel, to which the audio client is connected.
        /// </summary>
        /// <param name="guildId">Guild'd Id</param>
        /// <returns>Voice channel object</returns>
        IVoiceChannel GetGuildActiveVoiceChannel(ulong guildId);

        /// <summary>
        /// Returns guild's active audio client object.
        /// </summary>
        /// <param name="guildId">Guild'd Id</param>
        /// <returns>Audio client object</returns>
        IAudioClient GetGuildAudioClient(ulong guildId);

        /// <summary>
        /// Creates audio client and connects it to the voice channel.
        /// </summary>
        /// <param name="channel">Voice channel to connect to.</param>
        /// <returns>Connection task with connected audio client as a result.</returns>
        /// <exception cref="ArgumentException">The <paramref name="channel"/>'s guild already has active audio client.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="channel"/> was <see cref="null"/>.</exception>
        Task<IAudioClient> JoinChannelAsync(IVoiceChannel channel);

        /// <summary>
        /// Disconnects voice client from the voice channel.
        /// </summary>
        /// <param name="channel">Voice channel to dicconnect from.</param>
        /// <returns>Disconnection task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="channel"/> was <see cref="null"/>.</exception>
        /// <exception cref="ArgumentException">Given channel or it's guild doesn't have active voice channel.</exception>
        Task LeaveChannelAsync(IVoiceChannel channel);
    }

    /// <inheritdoc cref="IAudioClientManager"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ")]
    public sealed class AudioClientManager : IAudioClientManager
    {
        private readonly IDictionary<ulong, (IVoiceChannel Channel, IAudioClient Client)> _audioClientsCache; //Key: guild id 
        private readonly IDictionary<IAudioClient, IAudioPlayer> _audioPlayersCache;
        private readonly ILogger<AudioClientManager> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public AudioClientManager(ILogger<AudioClientManager> logger, ILoggerFactory loggerFactory)
        {
            _audioClientsCache = new Dictionary<ulong, (IVoiceChannel Channel, IAudioClient Client)>();
            _audioPlayersCache = new Dictionary<IAudioClient, IAudioPlayer>();
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public void Dispose()
        {
            foreach (var client in _audioPlayersCache)
                client.Value.Dispose();
            foreach (var entry in _audioClientsCache)
                entry.Value.Client.Dispose();
        }

        public Task<IAudioClient> JoinChannelAsync(IVoiceChannel channel)
        {
            return Task.Run(async () =>
            {
                if (channel is null)
                    throw new ArgumentNullException(nameof(channel));
                if (_audioClientsCache.ContainsKey(channel.Guild.Id))
                    throw new ArgumentException("Guild already has active audio client");

                var audioClient = await channel.ConnectAsync();
                _audioClientsCache.Add(channel.Guild.Id, (channel, audioClient));
                return audioClient;
            });
        }

        public Task LeaveChannelAsync(IVoiceChannel channel)
        {
            return Task.Run(async () =>
            {
                if (channel is null)
                    throw new ArgumentNullException(nameof(channel));
                if (!_audioClientsCache.ContainsKey(channel.Guild.Id))
                    throw new ArgumentException("Guild doesn't have active voice client");
                if (_audioClientsCache[channel.Guild.Id].Channel.Id != channel.Id)
                    throw new ArgumentException("Guild doesn't have active voice client on given channel.");

                await channel.DisconnectAsync();
                RemoveClientFromCache(channel.Guild);
            });
        }

        private void RemoveClientFromCache(IGuild guild)
        {
            if (_audioClientsCache.ContainsKey(guild.Id))
            {
                var audioClient = _audioClientsCache[guild.Id].Client;
                if (_audioPlayersCache.ContainsKey(audioClient))
                {
                    _audioPlayersCache[audioClient].Dispose();
                    var isPlayerRemoved = _audioPlayersCache.Remove(audioClient);
                    if (isPlayerRemoved)
                        _logger.LogDebug("Removed audio player instance for guild {guildName}", guild.Name);
                }
                var isClientRemoved = _audioClientsCache.Remove(guild.Id);
                if (isClientRemoved)
                    _logger.LogDebug("Removed audio client for guild {guildName}", guild.Name);
            }
        }

        public IAudioClient GetGuildAudioClient(ulong guildId) => _audioClientsCache[guildId].Client;
        
        public IVoiceChannel GetGuildActiveVoiceChannel(ulong guildId) => _audioClientsCache[guildId].Channel;

        public IAudioPlayer GetAudioPlayer(IAudioClient audioClient)
        {
            if (_audioPlayersCache.ContainsKey(audioClient))
                return _audioPlayersCache[audioClient];
            else
            {
                var player = new AudioPlayer(audioClient, 4096, _loggerFactory.CreateLogger<AudioPlayer>());
                _audioPlayersCache.Add(audioClient, player);
                return player;
            }
        }
    }
}
