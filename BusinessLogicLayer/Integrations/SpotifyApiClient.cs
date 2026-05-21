using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Integrations
{
    public class SpotifyApiSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = "http://localhost";
        public string ApiBaseUrl { get; set; } = "https://api.spotify.com/v1/";
        public string TokenEndpoint { get; set; } = "https://accounts.spotify.com/api/token";
    }

    public class SpotifyApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly SpotifyApiSettings _settings;

        public SpotifyApiClient(HttpClient httpClient, IOptions<SpotifyApiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
            using var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials"
                })
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await System.Text.Json.JsonDocument.ParseAsync(stream);
            if (document.RootElement.TryGetProperty("access_token", out var tokenProperty))
            {
                return tokenProperty.GetString() ?? string.Empty;
            }

            throw new InvalidOperationException("Spotify access token could not be retrieved.");
        }

        public async Task<IEnumerable<TrackDto>> SearchTracksAsync(string query, int limit = 10)
        {
            var token = await GetAccessTokenAsync();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"search?q={Uri.EscapeDataString(query)}&type=track&limit={limit}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await System.Text.Json.JsonDocument.ParseAsync(stream);

            if (!document.RootElement.TryGetProperty("tracks", out var tracksElement) || !tracksElement.TryGetProperty("items", out var items))
                return Array.Empty<TrackDto>();

            var results = new List<TrackDto>();
            foreach (var item in items.EnumerateArray())
            {
                var album = item.GetProperty("album");
                var artists = item.GetProperty("artists");
                var firstArtist = artists.EnumerateArray().FirstOrDefault();
                var artistName = firstArtist.ValueKind != System.Text.Json.JsonValueKind.Undefined
                    ? firstArtist.GetProperty("name").GetString() ?? string.Empty
                    : string.Empty;

                var releaseDateText = album.GetProperty("release_date").GetString() ?? string.Empty;
                var track = new TrackDto
                {
                    Provider = "Spotify",
                    ProviderTrackKey = item.GetProperty("id").GetString() ?? string.Empty,
                    Title = item.GetProperty("name").GetString() ?? string.Empty,
                    ArtistID = 0,
                    ArtistName = artistName,
                    Album = album.GetProperty("name").GetString() ?? string.Empty,
                    DurationMs = item.GetProperty("duration_ms").GetInt32(),
                    ReleaseDate = ParseReleaseDate(releaseDateText),
                    PreviewUrl = item.GetProperty("preview_url").GetString() ?? string.Empty,
                    SpotifyUrl = item.GetProperty("external_urls").GetProperty("spotify").GetString() ?? string.Empty
                };

                results.Add(track);
            }

            return results;
        }

        public async Task<string> GetTrackPreviewUrlAsync(string providerTrackKey)
        {
            var info = await GetTrackPreviewAndUrlAsync(providerTrackKey);
            return info.previewUrl;
        }

        public async Task<(string previewUrl, string spotifyUrl)> GetTrackPreviewAndUrlAsync(string providerTrackKey)
        {
            if (string.IsNullOrWhiteSpace(providerTrackKey))
                return (string.Empty, string.Empty);

            var token = await GetAccessTokenAsync();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"tracks/{Uri.EscapeDataString(providerTrackKey)}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await System.Text.Json.JsonDocument.ParseAsync(stream);
            var previewUrl = document.RootElement.GetProperty("preview_url").GetString() ?? string.Empty;
            var spotifyUrl = document.RootElement.GetProperty("external_urls").GetProperty("spotify").GetString() ?? string.Empty;
            return (previewUrl, spotifyUrl);
        }

        private static DateTime ParseReleaseDate(string releaseDateText)
        {
            if (DateTime.TryParse(releaseDateText, out var parsed))
                return parsed;

            if (releaseDateText.Length == 4 && int.TryParse(releaseDateText, out var year))
                return new DateTime(year, 1, 1);

            if (DateTime.TryParseExact(releaseDateText, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsed))
                return parsed;

            return DateTime.MinValue;
        }

        public Task<IEnumerable<TrackDto>> GetUserLibraryAsync(Guid userId)
        {
            return Task.FromResult<IEnumerable<TrackDto>>(Array.Empty<TrackDto>());
        }

        public Task<IEnumerable<PlayEventDto>> GetRecentPlaysAsync(Guid userId, DateTime since)
        {
            return Task.FromResult<IEnumerable<PlayEventDto>>(Array.Empty<PlayEventDto>());
        }

        public Task<AudioFeatureDto?> GetAudioFeaturesAsync(long trackId)
        {
            return Task.FromResult<AudioFeatureDto?>(null);
        }
    }
}
