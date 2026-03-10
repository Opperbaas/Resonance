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
            var song = new Song
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Artist = dto.Artist,
                Mood = dto.Mood,
                UserId = userId
            };
            await _uow.SongRepository.AddAsync(song);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<SongDto>> GetSongsForUserAsync(Guid userId)
        {
            var songs = await _uow.SongRepository.GetByUserIdAsync(userId);
            return songs.Select(s => new SongDto
            {
                Title = s.Title,
                Artist = s.Artist,
                Mood = s.Mood
            });
        }
    }
}