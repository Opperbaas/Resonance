using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface ISongRepository : IRepository<Track>
    {
        Task<IEnumerable<Track>> GetByUserIdAsync(Guid userId);
    }
}