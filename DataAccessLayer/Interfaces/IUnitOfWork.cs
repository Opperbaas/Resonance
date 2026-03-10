using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ISongRepository SongRepository { get; }
        Task<int> SaveChangesAsync();
    }
}