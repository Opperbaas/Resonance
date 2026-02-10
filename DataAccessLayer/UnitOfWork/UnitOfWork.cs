using System.Threading.Tasks;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Repositories;

namespace Resonance.DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ISpecificRepository _specificRepository;
        private IUserRepository _userRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ISpecificRepository SpecificRepository => _specificRepository ??= new SpecificRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}