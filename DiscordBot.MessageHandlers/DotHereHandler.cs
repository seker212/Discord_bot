﻿using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Helpers;
using DiscordBot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DiscordBot.MessageHandlers
{
    /// <summary>
    /// Mentions all users and roles with overrited permissions on whitelisted channel
    /// </summary>
    public class DotHereHandler : IMessageReceivedHandler
    {
        private const string DOT_HERE_PATTERN = @".here";
        private readonly ILogger<DotHereHandler> _logger;

        public DotHereHandler(ILogger<DotHereHandler> logger)
        {
            _logger = logger;
        }

        private bool IsApplicable(SocketMessage socketMessage)
            => socketMessage is IUserMessage &&
            socketMessage.Channel is SocketTextChannel &&
            (socketMessage.Content.StartsWith(DOT_HERE_PATTERN) || socketMessage.Content.EndsWith(DOT_HERE_PATTERN));

        private async Task RespondWithMentions(SocketMessage socketMessage)
        {
            var channel = (socketMessage.Channel as SocketTextChannel)!;
            IUserMessage userMessage = (socketMessage as IUserMessage)!;
            if (!channel.IsWhitelisted())
            {
                _logger.LogDebug(".here was attempted to be used in not whitelisted channel {ChannelName}", channel.Name);
                await userMessage.ReplyAsync("This channel is not whitelisted");
            }
            else
            {
                _logger.LogDebug("Generating .here message for channel {ChannelName}", channel.Name);
                var allowedPermisions = channel.PermissionOverwrites.Where(x => x.Permissions.ViewChannel == PermValue.Allow);
                var mentiones = allowedPermisions.Select(x => x.TargetType == PermissionTarget.Role ? MentionUtils.MentionRole(x.TargetId) : MentionUtils.MentionUser(x.TargetId));
                await userMessage.ReplyAsync(string.Join(" ", mentiones));
            }
        }

        public Task Execute(SocketMessage socketMessage)
            => Task.Run(async () =>
            {
                if (IsApplicable(socketMessage))
                    await RespondWithMentions(socketMessage);
            });
    }
}
