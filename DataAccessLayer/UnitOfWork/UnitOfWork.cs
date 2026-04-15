using System.Threading.Tasks;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Repositories;

namespace Resonance.DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IUserRepository _userRepository;
        private ISongRepository _songRepository;
        private IUserProfileRepository _profileRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public ISongRepository SongRepository => _songRepository ??= new SongRepository(_context);
        public IUserProfileRepository ProfileRepository => _profileRepository ??= new UserProfileRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}