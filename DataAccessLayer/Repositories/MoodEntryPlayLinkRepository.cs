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
    public class MoodEntryPlayLinkRepository : IMoodEntryPlayLinkRepository
    {
        private readonly ApplicationDbContext _context;

        public MoodEntryPlayLinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MoodEntryPlayLink entity)
        {
            await _context.Set<MoodEntryPlayLink>().AddAsync(entity);
        }

        public async Task<IEnumerable<MoodEntryPlayLink>> GetAllAsync()
        {
            return await _context.Set<MoodEntryPlayLink>().ToListAsync();
        }

        public async Task<MoodEntryPlayLink?> GetByIdAsync(object id)
        {
            return await _context.Set<MoodEntryPlayLink>().FindAsync(id);
        }

        public async Task<IEnumerable<MoodEntryPlayLink>> GetByMoodEntryIdAsync(long moodEntryId)
        {
            return await _context.Set<MoodEntryPlayLink>()
                .Where(link => link.MoodEntryID == moodEntryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MoodEntryPlayLink>> GetByPlayEventIdAsync(long playEventId)
        {
            return await _context.Set<MoodEntryPlayLink>()
                .Where(link => link.PlayEventID == playEventId)
                .ToListAsync();
        }

        public void Remove(MoodEntryPlayLink entity)
        {
            _context.Set<MoodEntryPlayLink>().Remove(entity);
        }

        public void Update(MoodEntryPlayLink entity)
        {
            _context.Set<MoodEntryPlayLink>().Update(entity);
        }
    }
}
