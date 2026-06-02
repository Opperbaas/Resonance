using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Integrations
{
    public class YouTubeApiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = "https://www.googleapis.com/youtube/v3/";
    }

    public class YouTubeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly YouTubeApiSettings _settings;

        public YouTubeApiClient(HttpClient httpClient, IOptions<YouTubeApiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<IEnumerable<TrackDto>> SearchVideosAsync(string query, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                return Array.Empty<TrackDto>();
            }

            var requestUri = $"search?part=snippet&type=video&maxResults={limit}&q={Uri.EscapeDataString(query)}&key={Uri.EscapeDataString(_settings.ApiKey)}";
            using var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);
            if (!document.RootElement.TryGetProperty("items", out var items))
            {
                return Array.Empty<TrackDto>();
            }

            var videoIds = items.EnumerateArray()
                .Select(item => item.GetProperty("id").GetProperty("videoId").GetString())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id!)
                .ToArray();

            var durations = await GetVideoDurationsAsync(videoIds);

            var results = new List<TrackDto>();
            foreach (var item in items.EnumerateArray())
            {
                var videoId = item.GetProperty("id").GetProperty("videoId").GetString();
                if (string.IsNullOrWhiteSpace(videoId))
                {
                    continue;
                }

                var snippet = item.GetProperty("snippet");
                var title = snippet.GetProperty("title").GetString() ?? string.Empty;
                var channelTitle = snippet.GetProperty("channelTitle").GetString() ?? string.Empty;
                var publishedAtText = snippet.GetProperty("publishedAt").GetString() ?? string.Empty;
                var releaseDate = ParsePublishedAt(publishedAtText);
                var durationMs = durations.TryGetValue(videoId, out var duration) ? duration : 0;

                results.Add(new TrackDto
                {
                    Provider = "YouTube",
                    ProviderTrackKey = videoId,
                    Title = title,
                    ArtistID = 0,
                    ArtistName = channelTitle,
                    Album = "YouTube",
                    DurationMs = durationMs,
                    ReleaseDate = releaseDate,
                    PreviewUrl = GetEmbedUrl(videoId),
                    ProviderUrl = GetWatchUrl(videoId)
                });
            }

            return results;
        }

        public Task<IEnumerable<TrackDto>> ImportUserLibraryAsync(Guid userId)
        {
            return Task.FromResult<IEnumerable<TrackDto>>(Array.Empty<TrackDto>());
        }

        public Task<IEnumerable<PlayEventDto>> ImportRecentPlaysAsync(Guid userId, DateTime since)
        {
            return Task.FromResult<IEnumerable<PlayEventDto>>(Array.Empty<PlayEventDto>());
        }

        public Task<AudioFeatureDto?> FetchAudioFeaturesAsync(long trackId)
        {
            return Task.FromResult<AudioFeatureDto?>(null);
        }

        public string GetEmbedUrl(string videoId)
        {
            return $"https://www.youtube.com/embed/{Uri.EscapeDataString(videoId)}?autoplay=1&rel=0";
        }

        public string GetWatchUrl(string videoId)
        {
            return $"https://www.youtube.com/watch?v={Uri.EscapeDataString(videoId)}";
        }

        private async Task<Dictionary<string, int>> GetVideoDurationsAsync(string[] videoIds)
        {
            if (videoIds.Length == 0)
                return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var requestUri = $"videos?part=contentDetails&id={string.Join(',', videoIds)}&key={Uri.EscapeDataString(_settings.ApiKey)}";
            using var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            if (!document.RootElement.TryGetProperty("items", out var items))
                return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var durations = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString() ?? string.Empty;
                var contentDetails = item.GetProperty("contentDetails");
                var duration = contentDetails.GetProperty("duration").GetString() ?? string.Empty;
                durations[id] = ParseDuration(duration);
            }

            return durations;
        }

        private static int ParseDuration(string duration)
        {
            try
            {
                var ts = System.Xml.XmlConvert.ToTimeSpan(duration);
                return (int)ts.TotalMilliseconds;
            }
            catch
            {
                return 0;
            }
        }

        private static DateTime ParsePublishedAt(string publishedAt)
        {
            if (DateTime.TryParse(publishedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            {
                return parsed;
            }

            return DateTime.MinValue;
        }
    }
}
