using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.BusinessLogicLayer.Services
{
    public class MoodEntryPlayLinkService : IMoodEntryPlayLinkService
    {
        private readonly IUnitOfWork _uow;

        public MoodEntryPlayLinkService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task AddLinkAsync(MoodEntryPlayLinkDto dto)
        {
            var link = new MoodEntryPlayLink
            {
                MoodEntryID = dto.MoodEntryID,
                PlayEventID = dto.PlayEventID,
                RelationType = dto.RelationType,
                WindowMinutes = dto.WindowMinutes
            };

            await _uow.MoodEntryPlayLinkRepository.AddAsync(link);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<MoodEntryPlayLinkDto>> GetLinksByMoodEntryAsync(long moodEntryId)
        {
            var links = await _uow.MoodEntryPlayLinkRepository.GetByMoodEntryIdAsync(moodEntryId);
            return links.Select(link => new MoodEntryPlayLinkDto
            {
                MoodEntryID = link.MoodEntryID,
                PlayEventID = link.PlayEventID,
                RelationType = link.RelationType,
                WindowMinutes = link.WindowMinutes
            });
        }

        public async Task<IEnumerable<MoodEntryPlayLinkDto>> GetLinksByPlayEventAsync(long playEventId)
        {
            var links = await _uow.MoodEntryPlayLinkRepository.GetByPlayEventIdAsync(playEventId);
            return links.Select(link => new MoodEntryPlayLinkDto
            {
                MoodEntryID = link.MoodEntryID,
                PlayEventID = link.PlayEventID,
                RelationType = link.RelationType,
                WindowMinutes = link.WindowMinutes
            });
        }
    }
}
