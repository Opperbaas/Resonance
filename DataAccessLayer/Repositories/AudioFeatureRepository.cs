using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Repositories
{
    public class AudioFeatureRepository : IAudioFeatureRepository
    {
        private readonly ApplicationDbContext _context;

        public AudioFeatureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AudioFeature entity)
        {
            await _context.AudioFeatures.AddAsync(entity);
        }

        public async Task<IEnumerable<AudioFeature>> GetAllAsync()
        {
            return await _context.AudioFeatures.ToListAsync();
        }

        public async Task<AudioFeature?> GetByIdAsync(object id)
        {
            return await _context.AudioFeatures.FindAsync(id);
        }

        public async Task<AudioFeature?> GetByTrackIdAsync(long trackId)
        {
            return await _context.AudioFeatures.FindAsync(trackId);
        }

        public void Remove(AudioFeature entity)
        {
            _context.AudioFeatures.Remove(entity);
        }

        public void Update(AudioFeature entity)
        {
            _context.AudioFeatures.Update(entity);
        }
    }
}
