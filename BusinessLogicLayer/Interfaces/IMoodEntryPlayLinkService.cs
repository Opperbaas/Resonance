using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IMoodEntryPlayLinkService
    {
        Task AddLinkAsync(MoodEntryPlayLinkDto dto);
        Task<IEnumerable<MoodEntryPlayLinkDto>> GetLinksByMoodEntryAsync(long moodEntryId);
        Task<IEnumerable<MoodEntryPlayLinkDto>> GetLinksByPlayEventAsync(long playEventId);
    }
}
