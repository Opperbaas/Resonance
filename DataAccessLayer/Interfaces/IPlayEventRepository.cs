using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IPlayEventRepository : IRepository<PlayEvent>
    {
        Task<IEnumerable<PlayEvent>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<PlayEvent>> GetByTrackIdAsync(long trackId);
    }
}
