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
    public class SongService : ISongService
    {
        private readonly IUnitOfWork _uow;

        public SongService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task AddSongAsync(SongDto dto, Guid userId)
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
            await _uow.SongRepository.AddAsync(track);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<SongDto>> GetSongsForUserAsync(Guid userId)
        {
            var tracks = await _uow.SongRepository.GetByUserIdAsync(userId);
            return tracks.Select(t => new SongDto
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