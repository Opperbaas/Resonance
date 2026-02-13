using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IItemRepository : IRepository<ItemEntity>
    {
        Task<ItemEntity> GetByNameAsync(string name);
    }
}