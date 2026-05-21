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
    public class PlayEventService : IPlayEventService
    {
        private readonly IUnitOfWork _uow;

        public PlayEventService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<long> AddPlayEventAsync(PlayEventDto dto)
        {
            var playEvent = new PlayEvent
            {
                UserID = dto.UserID,
                TrackID = dto.TrackID,
                SessionID = dto.SessionID,
                PlayedAt = dto.PlayedAt,
                PlayDurationMs = dto.PlayDurationMs,
                WasSkipped = dto.WasSkipped,
                Context = dto.Context
            };

            await _uow.PlayEventRepository.AddAsync(playEvent);
            await _uow.SaveChangesAsync();
            return playEvent.PlayEventID;
        }

        public async Task<PlayEventDto?> GetPlayEventAsync(long playEventId)
        {
            var playEvent = await _uow.PlayEventRepository.GetByIdAsync(playEventId);
            if (playEvent == null)
                return null;

            return new PlayEventDto
            {
                PlayEventID = playEvent.PlayEventID,
                UserID = playEvent.UserID,
                TrackID = playEvent.TrackID,
                SessionID = playEvent.SessionID,
                PlayedAt = playEvent.PlayedAt,
                PlayDurationMs = playEvent.PlayDurationMs,
                WasSkipped = playEvent.WasSkipped,
                Context = playEvent.Context
            };
        }

        public async Task<IEnumerable<PlayEventDto>> GetPlayEventsForTrackAsync(long trackId)
        {
            var playEvents = await _uow.PlayEventRepository.GetByTrackIdAsync(trackId);
            return playEvents.Select(evt => new PlayEventDto
            {
                PlayEventID = evt.PlayEventID,
                UserID = evt.UserID,
                TrackID = evt.TrackID,
                SessionID = evt.SessionID,
                PlayedAt = evt.PlayedAt,
                PlayDurationMs = evt.PlayDurationMs,
                WasSkipped = evt.WasSkipped,
                Context = evt.Context
            });
        }

        public async Task<IEnumerable<PlayEventDto>> GetPlayEventsForUserAsync(Guid userId)
        {
            var playEvents = await _uow.PlayEventRepository.GetByUserIdAsync(userId);
            return playEvents.Select(evt => new PlayEventDto
            {
                PlayEventID = evt.PlayEventID,
                UserID = evt.UserID,
                TrackID = evt.TrackID,
                SessionID = evt.SessionID,
                PlayedAt = evt.PlayedAt,
                PlayDurationMs = evt.PlayDurationMs,
                WasSkipped = evt.WasSkipped,
                Context = evt.Context
            });
        }
    }
}
