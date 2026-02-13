using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        Task<int> SaveChangesAsync();
    }
}