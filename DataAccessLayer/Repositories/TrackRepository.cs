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
    public class TrackRepository : ITrackRepository
    {
        private readonly ApplicationDbContext _context;

        public TrackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Track entity)
        {
            await _context.Tracks.AddAsync(entity);
        }

        public async Task<IEnumerable<Track>> GetAllAsync()
        {
            return await _context.Tracks.ToListAsync();
        }

        public async Task<Track?> GetByIdAsync(object id)
        {
            return await _context.Tracks.FindAsync(id);
        }

        public async Task<IEnumerable<Track>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Tracks.ToListAsync();
        }

        public void Remove(Track entity)
        {
            _context.Tracks.Remove(entity);
        }

        public void Update(Track entity)
        {
            _context.Tracks.Update(entity);
        }
    }
}