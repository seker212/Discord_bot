using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Provides token for discord client
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Value of the token
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Token's type
        /// </summary>
        TokenType TokenType { get; }
    }

    /// <inheritdoc cref="ITokenProvider"/>
    public class BotTokenProvider : ITokenProvider
    {
        public string Token => File.ReadAllText("token.txt");

        public TokenType TokenType => TokenType.Bot;
    }
}
