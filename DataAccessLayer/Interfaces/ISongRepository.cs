using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface ISongRepository : IRepository<Song>
    {
        Task<IEnumerable<Song>> GetByUserIdAsync(Guid userId);
    }
}