using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IAudioFeatureRepository : IRepository<AudioFeature>
    {
        Task<AudioFeature?> GetByTrackIdAsync(long trackId);
    }
}
