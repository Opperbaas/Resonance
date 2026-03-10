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
    public class SongRepository : ISongRepository
    {
        private readonly ApplicationDbContext _context;

        public SongRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Song entity)
        {
            await _context.Songs.AddAsync(entity);
        }

        public async Task<IEnumerable<Song>> GetAllAsync()
        {
            return await _context.Songs.ToListAsync();
        }

        public async Task<Song> GetByIdAsync(Guid id)
        {
            return await _context.Songs.FindAsync(id);
        }

        public async Task<IEnumerable<Song>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Songs.Where(s => s.UserId == userId).ToListAsync();
        }

        public void Remove(Song entity)
        {
            _context.Songs.Remove(entity);
        }

        public void Update(Song entity)
        {
            _context.Songs.Update(entity);
        }
    }
}