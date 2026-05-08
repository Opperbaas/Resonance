using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IMusicImportProvider
    {
        string ProviderName { get; }
        Task<IEnumerable<TrackDto>> ImportUserLibraryAsync(Guid userId);
        Task<IEnumerable<PlayEventDto>> ImportRecentPlaysAsync(Guid userId, DateTime since);
        Task<AudioFeatureDto?> FetchAudioFeaturesAsync(long trackId);
    }
}
