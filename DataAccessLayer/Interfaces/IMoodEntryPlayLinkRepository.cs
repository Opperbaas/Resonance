using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IMoodEntryPlayLinkRepository : IRepository<MoodEntryPlayLink>
    {
        Task<IEnumerable<MoodEntryPlayLink>> GetByMoodEntryIdAsync(long moodEntryId);
        Task<IEnumerable<MoodEntryPlayLink>> GetByPlayEventIdAsync(long playEventId);
    }
}
