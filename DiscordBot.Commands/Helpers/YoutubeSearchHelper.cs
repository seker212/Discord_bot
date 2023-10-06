
using DiscordBot.Commands.Helpers.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DiscordBot.Commands.Helpers
{
    /// <summary>
    /// Helper for geting youtube video data
    /// </summary>
    public interface IYoutubeSearchHelper
    {
        /// <summary>
        /// Method for getting youtube video data, checking if it is direct url or query.
        /// </summary>
        /// <param name="urlOrQuery">Text containg url to video or query</param>
        /// <returns>Youtube object containing information about video</returns>
        YoutubeVideoData GetYoutubeVideoData(string urlOrQuery);
    }

    /// <inheritdoc cref="IYoutubeSearchHelper"/>
    public class YoutubeSearchHelper : IYoutubeSearchHelper
    {
        private const string HTTP = "http://";
        private const string HTTPS = "https://";
        private readonly ILogger<YoutubeSearchHelper> _logger;

        public YoutubeSearchHelper(ILogger<YoutubeSearchHelper> logger) 
        { 
            _logger = logger;
        }

        public YoutubeVideoData GetYoutubeVideoData(string urlOrQuery)
        {
            var text = urlOrQuery.ToLower().Trim();

            if(text.StartsWith(HTTP) || text.StartsWith(HTTPS))
            {
                return GetVideoDataFromUri(urlOrQuery);
            }
            else
            {
                return GetVideoDataFromQuery(urlOrQuery);
            }
        }

        /// <summary>
        /// Method for resolving url to youtube video.
        /// </summary>
        /// <param name="uri">Correct uri to video</param>
        /// <returns>Youtube object containing information about video</returns>
        /// <exception cref="ArgumentException">Thrown if invalid uri was provided</exception>
        private YoutubeVideoData GetVideoDataFromUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                _logger.LogWarning("{Uri} was not a proper uri", uri);
                throw new ArgumentException($"{uri} was not a proprt uri", nameof(uri));
            }
            _logger.LogDebug("Getting yt metadata for {Uri}", uri);
            return GetVideoData(uri);
        }

        /// <summary>
        /// Method for resolving query to youtube video.
        /// </summary>
        /// <param name="query">Text query that will be used in youtube search</param>
        /// Youtube object containing information about video
        /// <returns></returns>
        private YoutubeVideoData GetVideoDataFromQuery(string query)
        {
            _logger.LogDebug($"Getting yt metadata for search query: \"{query}\"");
            return GetVideoData($"ytsearch1:\"{query}\"");
        }

        /// <summary>
        /// Method for resolving url or text (endQuery) using yt-dlp .
        /// </summary>
        /// <param name="endQuery">Formatted yt-dlp query</param>
        /// Youtube object containing information about video
        /// <returns></returns>
        private YoutubeVideoData GetVideoData(string endQuery)
        {
            using (var youtubeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"--print-json --skip-download -f 251/250/249 {endQuery}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            }))
            return JsonConvert.DeserializeObject<YoutubeVideoData>(youtubeProcess.StandardOutput.ReadToEnd())!;
        }
    }
}
