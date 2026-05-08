using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.BusinessLogicLayer.Integrations;

namespace Resonance.BusinessLogicLayer.Services
{
    public class SpotifyImportProvider : IMusicImportProvider
    {
        private readonly SpotifyApiClient _spotifyApiClient;

        public SpotifyImportProvider(SpotifyApiClient spotifyApiClient)
        {
            _spotifyApiClient = spotifyApiClient;
        }

        public string ProviderName => "Spotify";

        public async Task<IEnumerable<TrackDto>> ImportUserLibraryAsync(Guid userId)
        {
            return await _spotifyApiClient.GetUserLibraryAsync(userId);
        }

        public async Task<IEnumerable<PlayEventDto>> ImportRecentPlaysAsync(Guid userId, DateTime since)
        {
            return await _spotifyApiClient.GetRecentPlaysAsync(userId, since);
        }

        public async Task<AudioFeatureDto?> FetchAudioFeaturesAsync(long trackId)
        {
            return await _spotifyApiClient.GetAudioFeaturesAsync(trackId);
        }
    }
}
