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

        public Task<string> GetAccessTokenAsync()
        {
            // TODO: Implementeer hier de Spotify OAuth-tokenuitwisseling.
            // voor een productieklant heb je meestal een gebruikersaccess-token nodig,
            // geen Client Credentials-token, zodat je per gebruiker hun eigen library en plays kunt lezen.
            throw new NotImplementedException("Spotify access token retrieval is not implemented.");
        }

        public Task<IEnumerable<TrackDto>> GetUserLibraryAsync(Guid userId)
        {
            // TODO: roep Spotify API endpoints aan om de gebruikersbibliotheek op te halen.
            throw new NotImplementedException("Spotify user library fetch is not implemented.");
        }

        public Task<IEnumerable<PlayEventDto>> GetRecentPlaysAsync(Guid userId, DateTime since)
        {
            // TODO: roep Spotify API endpoints aan om de recente afspeelhistorie op te halen.
            throw new NotImplementedException("Spotify recent plays fetch is not implemented.");
        }

        public Task<AudioFeatureDto?> GetAudioFeaturesAsync(long trackId)
        {
            // TODO: roep Spotify API endpoints aan om audio features voor een track op te halen.
            throw new NotImplementedException("Spotify audio feature fetch is not implemented.");
        }
    }
}
