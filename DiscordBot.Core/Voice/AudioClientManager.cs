﻿using Discord;
using Discord.Audio;

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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ", Justification = "<Pending>")]
    public sealed class AudioClientManager : IAudioClientManager
    {
        private readonly IDictionary<ulong, (IVoiceChannel Channel, IAudioClient Client)> _audioClientsCache; //Key: guild id 
        private readonly IDictionary<IAudioClient, IAudioPlayer> _audioPlayersCache;

        public AudioClientManager()
        {
            _audioClientsCache = new Dictionary<ulong, (IVoiceChannel Channel, IAudioClient Client)>();
            _audioPlayersCache = new Dictionary<IAudioClient, IAudioPlayer>();
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

                var audioClient = _audioClientsCache[channel.Guild.Id].Client;
                if (_audioPlayersCache.ContainsKey(audioClient))
                    _audioPlayersCache.Remove(audioClient);

                await channel.DisconnectAsync();
                _audioClientsCache.Remove(channel.Guild.Id);
            });
        }

        public IAudioClient GetGuildAudioClient(ulong guildId) => _audioClientsCache[guildId].Client;
        
        public IVoiceChannel GetGuildActiveVoiceChannel(ulong guildId) => _audioClientsCache[guildId].Channel;

        public IAudioPlayer GetAudioPlayer(IAudioClient audioClient)
        {
            if (_audioPlayersCache.ContainsKey(audioClient))
                return _audioPlayersCache[audioClient];
            else
            {
                var player = new AudioPlayer(audioClient, 4096);
                _audioPlayersCache.Add(audioClient, player);
                return player;
            }
        }
    }
}
