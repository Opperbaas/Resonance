using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IAudioFeatureService
    {
        Task<AudioFeatureDto?> GetAudioFeatureAsync(long trackId);
        Task AddOrUpdateAudioFeatureAsync(AudioFeatureDto dto);
        Task RemoveAudioFeatureAsync(long trackId);
    }
}
