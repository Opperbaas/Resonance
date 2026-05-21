using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IMoodTypeRepository : IRepository<MoodType>
    {
        Task<IEnumerable<MoodType>> GetAllActiveAsync();
    }
}
