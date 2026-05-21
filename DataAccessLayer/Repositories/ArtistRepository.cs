using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly ApplicationDbContext _context;

        public ArtistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Artist entity)
        {
            await _context.Set<Artist>().AddAsync(entity);
        }

        public async Task<IEnumerable<Artist>> GetAllAsync()
        {
            return await _context.Set<Artist>().ToListAsync();
        }

        public async Task<Artist?> GetByIdAsync(object id)
        {
            return await _context.Set<Artist>().FindAsync(id);
        }

        public async Task<Artist?> GetByNameAsync(string name)
        {
            return await _context.Set<Artist>()
                .FirstOrDefaultAsync(a => a.Name == name);
        }

        public void Remove(Artist entity)
        {
            _context.Set<Artist>().Remove(entity);
        }

        public void Update(Artist entity)
        {
            _context.Set<Artist>().Update(entity);
        }
    }
}
