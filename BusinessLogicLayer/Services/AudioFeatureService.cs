using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.BusinessLogicLayer.Services
{
    public class AudioFeatureService : IAudioFeatureService
    {
        private readonly IUnitOfWork _uow;

        public AudioFeatureService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AudioFeatureDto?> GetAudioFeatureAsync(long trackId)
        {
            var feature = await _uow.AudioFeatureRepository.GetByTrackIdAsync(trackId);
            if (feature == null)
                return null;

            return new AudioFeatureDto
            {
                TrackID = feature.TrackID,
                Tempo = feature.Tempo,
                Energy = feature.Energy,
                Valence = feature.Valence,
                Danceability = feature.Danceability,
                Acousticness = feature.Acousticness,
                Loudness = feature.Loudness,
                FeatureSource = feature.FeatureSource,
                FeatureUpdatedAt = feature.FeatureUpdatedAt
            };
        }

        public async Task AddOrUpdateAudioFeatureAsync(AudioFeatureDto dto)
        {
            var existing = await _uow.AudioFeatureRepository.GetByTrackIdAsync(dto.TrackID);
            if (existing == null)
            {
                existing = new AudioFeature
                {
                    TrackID = dto.TrackID,
                    Tempo = dto.Tempo,
                    Energy = dto.Energy,
                    Valence = dto.Valence,
                    Danceability = dto.Danceability,
                    Acousticness = dto.Acousticness,
                    Loudness = dto.Loudness,
                    FeatureSource = dto.FeatureSource,
                    FeatureUpdatedAt = dto.FeatureUpdatedAt
                };
                await _uow.AudioFeatureRepository.AddAsync(existing);
            }
            else
            {
                existing.Tempo = dto.Tempo;
                existing.Energy = dto.Energy;
                existing.Valence = dto.Valence;
                existing.Danceability = dto.Danceability;
                existing.Acousticness = dto.Acousticness;
                existing.Loudness = dto.Loudness;
                existing.FeatureSource = dto.FeatureSource;
                existing.FeatureUpdatedAt = dto.FeatureUpdatedAt;
                _uow.AudioFeatureRepository.Update(existing);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task RemoveAudioFeatureAsync(long trackId)
        {
            var existing = await _uow.AudioFeatureRepository.GetByTrackIdAsync(trackId);
            if (existing == null)
                return;

            _uow.AudioFeatureRepository.Remove(existing);
            await _uow.SaveChangesAsync();
        }
    }
}
