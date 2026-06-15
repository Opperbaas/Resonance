using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface ITrackService
    {
        Task AddTrackAsync(TrackDto dto, Guid userId);
        Task<IEnumerable<TrackDto>> GetTracksForUserAsync(Guid userId);
        Task DeleteTrackAsync(long trackId);
    }
}