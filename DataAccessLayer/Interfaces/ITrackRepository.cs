using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface ITrackRepository : IRepository<Track>
    {
        Task<IEnumerable<Track>> GetByUserIdAsync(Guid userId);
    }
}