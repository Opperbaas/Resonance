using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.BusinessLogicLayer.Integrations;

namespace Resonance.BusinessLogicLayer.Services
{
    public class YouTubeImportProvider : IMusicImportProvider
    {
        private readonly YouTubeApiClient _youTubeApiClient;

        public YouTubeImportProvider(YouTubeApiClient youTubeApiClient)
        {
            _youTubeApiClient = youTubeApiClient;
        }

        public string ProviderName => "YouTube";

        public Task<IEnumerable<TrackDto>> ImportUserLibraryAsync(Guid userId)
        {
            // YouTube library import requires OAuth and is not implemented in this version.
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
    }
}
