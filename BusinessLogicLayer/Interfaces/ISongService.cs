using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface ISongService
    {
        Task AddSongAsync(SongDto dto, Guid userId);
        Task<IEnumerable<SongDto>> GetSongsForUserAsync(Guid userId);
    }
}