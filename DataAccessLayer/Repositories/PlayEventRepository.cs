using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Repositories
{
    public class PlayEventRepository : IPlayEventRepository
    {
        private readonly ApplicationDbContext _context;

        public PlayEventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PlayEvent entity)
        {
            await _context.Set<PlayEvent>().AddAsync(entity);
        }

        public async Task<IEnumerable<PlayEvent>> GetAllAsync()
        {
            return await _context.Set<PlayEvent>().ToListAsync();
        }

        public async Task<PlayEvent?> GetByIdAsync(object id)
        {
            return await _context.Set<PlayEvent>().FindAsync(id);
        }

        public async Task<IEnumerable<PlayEvent>> GetByTrackIdAsync(long trackId)
        {
            return await _context.Set<PlayEvent>()
                .Where(evt => evt.TrackID == trackId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayEvent>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Set<PlayEvent>()
                .Where(evt => evt.UserID == userId)
                .ToListAsync();
        }

        public void Remove(PlayEvent entity)
        {
            _context.Set<PlayEvent>().Remove(entity);
        }

        public void Update(PlayEvent entity)
        {
            _context.Set<PlayEvent>().Update(entity);
        }
    }
}
