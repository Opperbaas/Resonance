using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.BusinessLogicLayer.Services
{
    public class MoodEntryService : IMoodEntryService
    {
        private readonly IUnitOfWork _uow;

        public MoodEntryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<long> AddMoodEntryAsync(MoodEntryDto dto)
        {
            var entry = new MoodEntry
            {
                UserID = dto.UserID,
                MoodTypeID = dto.MoodTypeID,
                OccurredAt = dto.OccurredAt,
                Note = dto.Note,
                ContextTag = dto.ContextTag,
                Source = dto.Source
            };

            await _uow.MoodEntryRepository.AddAsync(entry);
            await _uow.SaveChangesAsync();
            return entry.MoodEntryID;
        }

        public async Task<MoodEntryDto?> GetMoodEntryAsync(long moodEntryId)
        {
            var entry = await _uow.MoodEntryRepository.GetByIdAsync(moodEntryId);
            if (entry == null)
                return null;

            return new MoodEntryDto
            {
                MoodEntryID = entry.MoodEntryID,
                UserID = entry.UserID,
                MoodTypeID = entry.MoodTypeID,
                OccurredAt = entry.OccurredAt,
                Note = entry.Note,
                ContextTag = entry.ContextTag,
                Source = entry.Source
            };
        }

        public async Task UpdateMoodEntryAsync(MoodEntryDto dto)
        {
            var entry = await _uow.MoodEntryRepository.GetByIdAsync(dto.MoodEntryID);
            if (entry == null)
                return;

            entry.MoodTypeID = dto.MoodTypeID;
            entry.Note = dto.Note;
            entry.OccurredAt = dto.OccurredAt;
            entry.ContextTag = dto.ContextTag;
            entry.Source = dto.Source;
            _uow.MoodEntryRepository.Update(entry);
            await _uow.SaveChangesAsync();
        }

        public async Task<MoodEntryDto?> GetLatestMoodEntryForTrackAsync(Guid userId, long trackId)
        {
            var entries = await _uow.MoodEntryRepository.GetByUserIdAsync(userId);
            var latest = entries
                .Where(entry => !string.IsNullOrWhiteSpace(entry.ContextTag) && entry.ContextTag.Equals($"Track:{trackId}", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(entry => entry.OccurredAt)
                .FirstOrDefault();

            if (latest == null)
                return null;

            return new MoodEntryDto
            {
                MoodEntryID = latest.MoodEntryID,
                UserID = latest.UserID,
                MoodTypeID = latest.MoodTypeID,
                OccurredAt = latest.OccurredAt,
                Note = latest.Note,
                ContextTag = latest.ContextTag,
                Source = latest.Source
            };
        }

        public async Task<IEnumerable<MoodEntryDto>> GetMoodEntriesForUserAsync(Guid userId)
        {
            var entries = await _uow.MoodEntryRepository.GetByUserIdAsync(userId);
            return entries.Select(entry => new MoodEntryDto
            {
                MoodEntryID = entry.MoodEntryID,
                UserID = entry.UserID,
                MoodTypeID = entry.MoodTypeID,
                OccurredAt = entry.OccurredAt,
                Note = entry.Note,
                ContextTag = entry.ContextTag,
                Source = entry.Source
            });
        }
    }
}
