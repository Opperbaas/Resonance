using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Services;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;
using Xunit;

namespace Resonance.Tests
{
    public class ProfileAndTrackServiceTests
    {
        [Fact]
        public async Task TrackService_AddTrackAsync_UsesExistingArtist_WhenArtistExists()
        {
            var artist = new Artist { ArtistID = 1, Name = "Test Artist" };
            var trackRepo = new FakeTrackRepository();
            var unitOfWork = new FakeUnitOfWork
            {
                ArtistRepository = new FakeArtistRepository(new[] { artist }),
                TrackRepository = trackRepo
            };

            var service = new TrackService(unitOfWork);
            await service.AddTrackAsync(new TrackDto
            {
                Provider = "Spotify",
                ProviderTrackKey = "track123",
                Title = "Song A",
                ArtistName = "Test Artist",
                Album = "Album X",
                DurationMs = 1000,
                ReleaseDate = DateTime.UtcNow
            }, Guid.NewGuid());

            Assert.Single(trackRepo.Items);
            Assert.Equal(1, trackRepo.Items.First().ArtistID);
        }

        [Fact]
        public async Task TrackService_DeleteTrackAsync_RemovesTrack_WhenExists()
        {
            var track = new Track { TrackID = 1, Title = "Song A" };
            var trackRepo = new FakeTrackRepository(new[] { track });
            var unitOfWork = new FakeUnitOfWork { TrackRepository = trackRepo };
            var service = new TrackService(unitOfWork);

            await service.DeleteTrackAsync(1);

            Assert.Empty(trackRepo.Items);
        }

        [Fact]
        public async Task ProfileService_IsUsernameTakenAsync_ReturnsTrue_WhenUsernameTakenByOther()
        {
            var existingUser = new User { Id = Guid.NewGuid(), Username = "existing", Email = "x@x.com" };
            var unitOfWork = new FakeUnitOfWork
            {
                UserRepository = new FakeUserRepository(new[] { existingUser })
            };
            var service = new ProfileService(unitOfWork);

            var result = await service.IsUsernameTakenAsync("existing", Guid.NewGuid());

            Assert.True(result);
        }

        [Fact]
        public async Task ProfileService_UpdateUsernameAsync_UpdatesUsername_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "oldname", Email = "test@test.com" };
            var unitOfWork = new FakeUnitOfWork
            {
                UserRepository = new FakeUserRepository(new[] { user })
            };
            var service = new ProfileService(unitOfWork);

            var success = await service.UpdateUsernameAsync(userId, "newname");

            Assert.True(success);
            Assert.Equal("newname", user.Username);
        }

        [Fact]
        public async Task ProfileService_GetProfileAsync_ReturnsProfileData_WhenProfileExists()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "username", Email = "email@test.com" };
            var profile = new UserProfile { UserId = userId, PreferredLocale = "nl", TimeZone = "Europe/Amsterdam", PrivacyLevel = PrivacyLevel.Public };
            var unitOfWork = new FakeUnitOfWork
            {
                UserRepository = new FakeUserRepository(new[] { user }),
                ProfileRepository = new FakeUserProfileRepository(new[] { profile })
            };
            var service = new ProfileService(unitOfWork);

            var result = await service.GetProfileAsync(userId);

            Assert.NotNull(result);
            Assert.Equal("username", result.Username);
            Assert.Equal("email@test.com", result.Email);
            Assert.Equal("nl", result.PreferredLocale);
            Assert.Equal("Europe/Amsterdam", result.TimeZone);
            Assert.Equal(PrivacyLevel.Public, result.PrivacyLevel);
        }
    }

    internal class FakeUnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepository { get; set; } = null!;
        public ITrackRepository TrackRepository { get; set; } = null!;
        public IArtistRepository ArtistRepository { get; set; } = null!;
        public IMoodTypeRepository MoodTypeRepository { get; set; } = new FakeMoodTypeRepository();
        public IAudioFeatureRepository AudioFeatureRepository { get; set; } = new FakeAudioFeatureRepository();
        public IUserProfileRepository ProfileRepository { get; set; } = null!;
        public IMoodEntryRepository MoodEntryRepository { get; set; } = new FakeMoodEntryRepository();
        public IMoodEntryPlayLinkRepository MoodEntryPlayLinkRepository { get; set; } = new FakeMoodEntryPlayLinkRepository();
        public IPlayEventRepository PlayEventRepository { get; set; } = new FakePlayEventRepository();

        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }

    internal class FakeTrackRepository : FakeRepository<Track>, ITrackRepository
    {
        public FakeTrackRepository() { }
        public FakeTrackRepository(IEnumerable<Track> items) : base(items) { }
        public Task<IEnumerable<Track>> GetByUserIdAsync(Guid userId) => Task.FromResult<IEnumerable<Track>>(Items);
        public override Task<Track?> GetByIdAsync(object id)
        {
            if (id is long longId)
            {
                return Task.FromResult(Items.FirstOrDefault(x => x.TrackID == longId));
            }
            return Task.FromResult<Track?>(null);
        }
    }

    internal class FakeArtistRepository : FakeRepository<Artist>, IArtistRepository
    {
        public FakeArtistRepository(IEnumerable<Artist> items) : base(items) { }
        public Task<Artist?> GetByNameAsync(string name) => Task.FromResult(Items.FirstOrDefault(x => x.Name == name));
    }

    internal class FakeUserRepository : FakeRepository<User>, IUserRepository
    {
        public FakeUserRepository(IEnumerable<User> items) : base(items) { }
        public Task<User?> GetByUsernameAsync(string username) => Task.FromResult(Items.FirstOrDefault(x => x.Username == username));
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult(Items.FirstOrDefault(x => x.Email == email));
        public Task<User?> GetByResetTokenAsync(string token) => Task.FromResult(Items.FirstOrDefault(x => x.PasswordResetToken == token));
        public override Task<User?> GetByIdAsync(object id)
        {
            if (id is Guid guid)
            {
                return Task.FromResult(Items.FirstOrDefault(x => x.Id == guid));
            }
            return Task.FromResult<User?>(null);
        }
    }

    internal class FakeUserProfileRepository : FakeRepository<UserProfile>, IUserProfileRepository
    {
        public FakeUserProfileRepository(IEnumerable<UserProfile> items) : base(items) { }
        public Task<UserProfile?> GetByUserIdAsync(Guid userId) => Task.FromResult(Items.FirstOrDefault(x => x.UserId == userId));
    }

    internal class FakeRepository<T> : IRepository<T> where T : class
    {
        protected readonly List<T> Data;
        public FakeRepository() => Data = new List<T>();
        public FakeRepository(IEnumerable<T> items) => Data = new List<T>(items);
        public IEnumerable<T> Items => Data;
        public Task AddAsync(T entity)
        {
            Data.Add(entity);
            return Task.CompletedTask;
        }
        public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult<IEnumerable<T>>(Data);
        public virtual Task<T?> GetByIdAsync(object id)
        {
            return Task.FromResult<T?>(null);
        }
        public void Remove(T entity) => Data.Remove(entity);
        public void Update(T entity)
        {
            // in-memory reference update
        }
    }

    internal class FakeMoodTypeRepository : FakeRepository<MoodType>, IMoodTypeRepository
    {
        public Task<IEnumerable<MoodType>> GetAllActiveAsync() => Task.FromResult<IEnumerable<MoodType>>(Items);
    }

    internal class FakeAudioFeatureRepository : FakeRepository<AudioFeature>, IAudioFeatureRepository
    {
        public Task<AudioFeature?> GetByTrackIdAsync(long trackId) => Task.FromResult(Items.FirstOrDefault(x => x.TrackID == trackId));
    }

    internal class FakeMoodEntryRepository : FakeRepository<MoodEntry>, IMoodEntryRepository
    {
        public Task<IEnumerable<MoodEntry>> GetByUserIdAsync(Guid userId) => Task.FromResult<IEnumerable<MoodEntry>>(Items.Where(x => x.UserID == userId));
    }

    internal class FakeMoodEntryPlayLinkRepository : FakeRepository<MoodEntryPlayLink>, IMoodEntryPlayLinkRepository
    {
        public Task<IEnumerable<MoodEntryPlayLink>> GetByMoodEntryIdAsync(long moodEntryId) => Task.FromResult<IEnumerable<MoodEntryPlayLink>>(Items.Where(x => x.MoodEntryID == moodEntryId));
        public Task<IEnumerable<MoodEntryPlayLink>> GetByPlayEventIdAsync(long playEventId) => Task.FromResult<IEnumerable<MoodEntryPlayLink>>(Items.Where(x => x.PlayEventID == playEventId));
    }

    internal class FakePlayEventRepository : FakeRepository<PlayEvent>, IPlayEventRepository
    {
        public Task<IEnumerable<PlayEvent>> GetByUserIdAsync(Guid userId) => Task.FromResult<IEnumerable<PlayEvent>>(Items.Where(x => x.UserID == userId));
        public Task<IEnumerable<PlayEvent>> GetByTrackIdAsync(long trackId) => Task.FromResult<IEnumerable<PlayEvent>>(Items.Where(x => x.TrackID == trackId));
    }
}
