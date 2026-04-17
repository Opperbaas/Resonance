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
    public class MoodEntryRepository : IMoodEntryRepository
    {
        private readonly ApplicationDbContext _context;

        public MoodEntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MoodEntry entity)
        {
            await _context.Set<MoodEntry>().AddAsync(entity);
        }

        public async Task<IEnumerable<MoodEntry>> GetAllAsync()
        {
            return await _context.Set<MoodEntry>().ToListAsync();
        }

        public async Task<MoodEntry?> GetByIdAsync(object id)
        {
            return await _context.Set<MoodEntry>().FindAsync(id);
        }

        public async Task<IEnumerable<MoodEntry>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Set<MoodEntry>()
                .Where(entry => entry.UserID == userId)
                .ToListAsync();
        }

        public void Remove(MoodEntry entity)
        {
            _context.Set<MoodEntry>().Remove(entity);
        }

        public void Update(MoodEntry entity)
        {
            _context.Set<MoodEntry>().Update(entity);
        }
    }
}
