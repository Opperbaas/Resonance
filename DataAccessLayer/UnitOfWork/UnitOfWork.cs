using System.Threading.Tasks;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Repositories;

namespace Resonance.DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IUserRepository? _userRepository;
        private ITrackRepository? _trackRepository;
        private IArtistRepository? _artistRepository;
        private IMoodTypeRepository? _moodTypeRepository;
        private IAudioFeatureRepository? _audioFeatureRepository;
        private IUserProfileRepository? _profileRepository;
        private IMoodEntryRepository? _moodEntryRepository;
        private IMoodEntryPlayLinkRepository? _moodEntryPlayLinkRepository;
        private IPlayEventRepository? _playEventRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public ITrackRepository TrackRepository => _trackRepository ??= new TrackRepository(_context);
        public IArtistRepository ArtistRepository => _artistRepository ??= new ArtistRepository(_context);
        public IMoodTypeRepository MoodTypeRepository => _moodTypeRepository ??= new MoodTypeRepository(_context);
        public IAudioFeatureRepository AudioFeatureRepository => _audioFeatureRepository ??= new AudioFeatureRepository(_context);
        public IUserProfileRepository ProfileRepository => _profileRepository ??= new UserProfileRepository(_context);
        public IMoodEntryRepository MoodEntryRepository => _moodEntryRepository ??= new MoodEntryRepository(_context);
        public IMoodEntryPlayLinkRepository MoodEntryPlayLinkRepository => _moodEntryPlayLinkRepository ??= new MoodEntryPlayLinkRepository(_context);
        public IPlayEventRepository PlayEventRepository => _playEventRepository ??= new PlayEventRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}