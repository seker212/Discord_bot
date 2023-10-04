
using DiscordBot.Commands.Helpers.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DiscordBot.Commands.Helpers
{
    public interface IYoutubeSearchHelper
    {
        YoutubeVideoData GetYoutubeVideoData(string urlOrQuery);
    }

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

        private YoutubeVideoData GetVideoDataFromQuery(string query)
        {
            _logger.LogDebug($"Getting yt metadata for search query: \"{query}\"");
            return GetVideoData($"ytsearch1:\"{query}\"");
        }

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
