using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface ISpecificRepository : IRepository<SpecificEntity>
    {
        Task<SpecificEntity> GetByNameAsync(string name);
    }
}