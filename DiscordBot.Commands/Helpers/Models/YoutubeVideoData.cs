using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.Helpers.Models
{
    public class YoutubeVideoData
    {
        public YoutubeVideoData(string title, string description, string thumbnailUrl, string youtubeUrl, string downloadUrl, string duration)
        {
            Title = title;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            YoutubeUrl = youtubeUrl;
            DownloadUrl = downloadUrl;
            Duration = duration;
        }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("description")]
        public string Description { get; }

        [JsonProperty("thumbnail")]
        public string ThumbnailUrl { get; }

        [JsonProperty("webpage_url")]
        public string YoutubeUrl { get; }

        [JsonProperty("url")]
        public string DownloadUrl { get; }

        [JsonProperty("duration_string")]
        public string Duration { get; }
    }
}
