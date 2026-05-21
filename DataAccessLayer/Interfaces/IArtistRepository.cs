using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IArtistRepository : IRepository<Artist>
    {
        Task<Artist?> GetByNameAsync(string name);
    }
}
