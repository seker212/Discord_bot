using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
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
        public string Token => Environment.GetEnvironmentVariable("TOKEN") ?? GetTokenFromFile() ?? throw new ArgumentNullException("Could not find TOKEN environment variable's value");

        public TokenType TokenType => TokenType.Bot;

        private string? GetTokenFromFile() // For dev purposes only
        {
            var fileName = "token.txt";
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);

            var currentDirPath = Directory.GetCurrentDirectory();
            var match = Regex.Match(currentDirPath, @"(.*)\\bin\\");
            if (match.Success)
            {
                var dir = new DirectoryInfo(match.Groups[1].Value);
                var dirFiles = dir.GetFiles();
                if (dirFiles.Any(x => x.Name.EndsWith(".csproj")) && dirFiles.Any(x => x.Name == fileName))
                    return File.ReadAllText(dir.GetFiles().Single(x => x.Name == fileName).FullName);
                dir = dir.Parent;
                if (dir is not null)
                {
                    dirFiles = dir.GetFiles();
                    if (dirFiles.Any(x => x.Name.EndsWith(".sln")) && dirFiles.Any(x => x.Name == fileName))
                        return File.ReadAllText(dir.GetFiles().Single(x => x.Name == fileName).FullName);
                }
            }
            return null;
        }
    }
}
