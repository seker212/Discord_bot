namespace DiscordBot.Commands.Core
{
    public enum CommandOptionType
    {
        /// <summary>
        /// A string of text.
        /// </summary>
        String,
        /// <summary>
        /// An integer.
        /// </summary>
        Integer,
        /// <summary>
        /// A bool.
        /// </summary>
        Boolean,
        /// <summary>
        /// A guild channel.
        /// </summary>
        GuildChannel,
        /// <summary>
        /// A guild text channel.
        /// </summary>
        GuildTextChannel,
        /// <summary>
        /// A guild voice channel.
        /// </summary>
        GuildVoiceChannel,
        /// <summary>
        /// A user. <see cref="Discord.IUser"/>
        /// </summary>
        User,
        /// <summary>
        /// A guild role. <see cref="Discord.IRole"/>
        /// </summary>
        Role,
        /// <summary>
        /// A user or a guild role. <see cref="Discord.IUser"/> or <see cref="Discord.IRole"/>
        /// </summary>
        UserOrRole,
        /// <summary>
        /// <see cref="double"/>
        /// </summary>
        Number,
        /// <summary>
        /// An attachment. <see cref="Discord.IAttachment"/>
        /// </summary>
        Attachment
    }
}
