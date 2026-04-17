using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IMoodEntryService
    {
        Task AddMoodEntryAsync(MoodEntryDto dto);
        Task<IEnumerable<MoodEntryDto>> GetMoodEntriesForUserAsync(Guid userId);
        Task<MoodEntryDto?> GetMoodEntryAsync(long moodEntryId);
    }
}
