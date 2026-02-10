using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Repositories
{
    public class SpecificRepository : Repository<SpecificEntity>, ISpecificRepository
    {
        public SpecificRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<SpecificEntity> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Name == name);
        }
    }
}