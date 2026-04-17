using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IPlayEventService
    {
        Task AddPlayEventAsync(PlayEventDto dto);
        Task<IEnumerable<PlayEventDto>> GetPlayEventsForUserAsync(Guid userId);
        Task<IEnumerable<PlayEventDto>> GetPlayEventsForTrackAsync(long trackId);
        Task<PlayEventDto?> GetPlayEventAsync(long playEventId);
    }
}
