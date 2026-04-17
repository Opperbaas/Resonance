using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ISongRepository SongRepository { get; }
        IUserProfileRepository ProfileRepository { get; }
        IMoodEntryRepository MoodEntryRepository { get; }
        IMoodEntryPlayLinkRepository MoodEntryPlayLinkRepository { get; }
        IPlayEventRepository PlayEventRepository { get; }
        Task<int> SaveChangesAsync();
    }
}