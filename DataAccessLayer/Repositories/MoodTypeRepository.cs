using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Repositories
{
    public class MoodTypeRepository : IMoodTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public MoodTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MoodType entity)
        {
            await _context.Set<MoodType>().AddAsync(entity);
        }

        public async Task<IEnumerable<MoodType>> GetAllAsync()
        {
            return await _context.Set<MoodType>().ToListAsync();
        }

        public async Task<IEnumerable<MoodType>> GetAllActiveAsync()
        {
            return await _context.Set<MoodType>().Where(mt => mt.IsActive).ToListAsync();
        }

        public async Task<MoodType?> GetByIdAsync(object id)
        {
            return await _context.Set<MoodType>().FindAsync(id);
        }

        public void Remove(MoodType entity)
        {
            _context.Set<MoodType>().Remove(entity);
        }

        public void Update(MoodType entity)
        {
            _context.Set<MoodType>().Update(entity);
        }
    }
}
