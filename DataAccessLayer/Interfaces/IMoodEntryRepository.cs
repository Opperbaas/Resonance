using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IMoodEntryRepository : IRepository<MoodEntry>
    {
        Task<IEnumerable<MoodEntry>> GetByUserIdAsync(Guid userId);
    }
}
