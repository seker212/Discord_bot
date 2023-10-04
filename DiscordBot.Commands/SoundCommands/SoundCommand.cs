﻿using Discord;
using Discord.WebSocket;
using DiscordBot.Commands.Core;
using DiscordBot.Commands.Core.CommandAttributes;
using DiscordBot.Commands.Helpers;
using DiscordBot.Core.Voice;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.SoundCommands
{
    [Name("sound")]
    [Description("Plays sound")]
    [Option("soundname", "sound name", CommandOptionType.String)]
    [Option("channel", "voice channel", CommandOptionType.GuildVoiceChannel, false)]
    public class SoundCommand : AbstractSoundCommand
    {
        private readonly IAudioQueueManager _audioQueueManager;
        private readonly IAudioClientManager _audioClientManager;
        private readonly IAudioStreamHelper _audioStreamHelper;
        private readonly IVoiceChannelResolver _voiceChannelResolver;
        private readonly ILogger<SoundCommand> _logger;

        public SoundCommand(IAudioClientManager audioClientManager, ILogger<SoundCommand> logger, IAudioQueueManager audioQueueManager, IAudioStreamHelper audioStreamHelper, IVoiceChannelResolver voiceChannelResolver)
        {
            _audioClientManager = audioClientManager;
            _logger = logger;
            _audioQueueManager = audioQueueManager;
            _audioStreamHelper = audioStreamHelper;
            _voiceChannelResolver = voiceChannelResolver;
        }

        public override async Task ExecuteAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();
            
            try
            {
                var soundName = command.GetRequiredOptionValue<string>("soundname");
                var audioFile = new FileInfo(Path.Combine(AudioDirectoryPath, $"{soundName}.mp3"));
                _logger.LogDebug("Searching audio file in {dir} directory", audioFile.Directory?.FullName);
                if (!audioFile.Exists)
                {
                    var message = $"Sound {soundName} not found";
                    if (command.HasResponded)
                        await command.ModifyOriginalResponseAsync(m => m.Content = message);
                    else
                        await command.RespondAsync(message);
                    throw new FileNotFoundException(message, audioFile.FullName);
                }
                else
                {
                    var targetChannel = _voiceChannelResolver.GetVoiceChannel(command);
                    var queueCount = _audioQueueManager.GetQueueCount(command.GuildId.Value);

                    if(targetChannel == null)
                    {
                        throw new Exception("Target channel is null");
                    }

                    var queueEntry = new AudioQueueEntry(
                        targetChannel,
                        new Lazy<AudioStreamElements>(() => _audioStreamHelper.CreateAudioStream(audioFile)),
                        () => SendNowPlaying(soundName, command, queueCount == 0),
                        () => _logger.LogDebug("Finished playing"),
                        new Dictionary<string, object?>() { { "CommandCallID", command.Id } }
                        );
                    var queuePosition = _audioQueueManager.Add(command.GuildId.Value, queueEntry);
                    if (queueCount > 0)
                        await command.ModifyOriginalResponseAsync(m => { m.Content = $"Added to queue on {queuePosition - 1} position"; });                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sound command threw an exception");
            }
         
        }

        private void SendNowPlaying(string soundName, SocketSlashCommand command, bool isResponse)
        {
            var message = $"Playing sound {soundName}";
            if (isResponse)
                command.ModifyOriginalResponseAsync(m => { m.Content = message; });
            else
                command.Channel.SendMessageAsync(message);
        }
    }
}
