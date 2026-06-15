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
            long artistId = dto.ArtistID;
            if (artistId <= 0 && !string.IsNullOrWhiteSpace(dto.ArtistName))
            {
                var artistName = dto.ArtistName.Trim();
                var existingArtist = await _uow.ArtistRepository.GetByNameAsync(artistName);
                if (existingArtist == null)
                {
                    var newArtist = new Artist { Name = artistName };
                    await _uow.ArtistRepository.AddAsync(newArtist);
                    await _uow.SaveChangesAsync();
                    artistId = newArtist.ArtistID;
                }
                else
                {
                    artistId = existingArtist.ArtistID;
                }
            }
            else if (artistId > 0)
            {
                var existingArtist = await _uow.ArtistRepository.GetByIdAsync(artistId);
                if (existingArtist == null)
                {
                    throw new InvalidOperationException($"Artist with ID {artistId} does not exist.");
                }
            }
            else
            {
                var unknownArtist = await _uow.ArtistRepository.GetByNameAsync("Unknown Artist");
                if (unknownArtist == null)
                {
                    unknownArtist = new Artist { Name = "Unknown Artist" };
                    await _uow.ArtistRepository.AddAsync(unknownArtist);
                    await _uow.SaveChangesAsync();
                }
                artistId = unknownArtist.ArtistID;
            }

            var track = new Track
            {
                Provider = dto.Provider,
                ProviderTrackKey = dto.ProviderTrackKey,
                Title = dto.Title,
                ArtistID = artistId,
                Album = dto.Album,
                DurationMs = dto.DurationMs,
                ReleaseDate = dto.ReleaseDate
            };

            await _uow.TrackRepository.AddAsync(track);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteTrackAsync(long trackId)
        {
            var track = await _uow.TrackRepository.GetByIdAsync(trackId);
            if (track == null)
                return;

            _uow.TrackRepository.Remove(track);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<TrackDto>> GetTracksForUserAsync(Guid userId)
        {
            var tracks = await _uow.TrackRepository.GetByUserIdAsync(userId);
            return tracks.Select(t => new TrackDto
            {
                TrackID = t.TrackID,
                Provider = t.Provider,
                ProviderTrackKey = t.ProviderTrackKey,
                Title = t.Title,
                ArtistID = t.ArtistID,
                ArtistName = string.Empty,
                Album = t.Album,
                DurationMs = t.DurationMs,
                ReleaseDate = t.ReleaseDate
            });
        }
    }
}