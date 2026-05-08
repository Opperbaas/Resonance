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
    public class TrackService : ITrackService
    {
        private readonly IUnitOfWork _uow;

        public TrackService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task AddTrackAsync(TrackDto dto, Guid userId)
        {
            var track = new Track
            {
                Provider = dto.Provider,
                ProviderTrackKey = dto.ProviderTrackKey,
                Title = dto.Title,
                ArtistID = dto.ArtistID,
                Album = dto.Album,
                DurationMs = dto.DurationMs,
                ReleaseDate = dto.ReleaseDate
            };
            await _uow.TrackRepository.AddAsync(track);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<TrackDto>> GetTracksForUserAsync(Guid userId)
        {
            var tracks = await _uow.TrackRepository.GetByUserIdAsync(userId);
            return tracks.Select(t => new TrackDto
            {
                Provider = t.Provider,
                ProviderTrackKey = t.ProviderTrackKey,
                Title = t.Title,
                ArtistID = t.ArtistID,
                Album = t.Album,
                DurationMs = t.DurationMs,
                ReleaseDate = t.ReleaseDate
            });
        }
    }
}