using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        ISpecificRepository SpecificRepository { get; }
        Task<int> SaveChangesAsync();
    }
}