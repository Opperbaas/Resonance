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
    public class PlayEventServiceTests
    {
        [Fact]
        public async Task AddPlayEventAsync_SavesNewPlayEvent_WhenDtoIsValid()
        {
            var playEventRepo = new PlayEventServiceFakePlayEventRepository();
            var unitOfWork = new PlayEventServiceFakeUnitOfWork { PlayEventRepository = playEventRepo };
            var service = new PlayEventService(unitOfWork);

            var dto = new PlayEventDto
            {
                UserID = Guid.NewGuid(),
                TrackID = 42,
                SessionID = 10,
                PlayedAt = DateTime.UtcNow,
                PlayDurationMs = 12345,
                WasSkipped = false,
                Context = "Playback"
            };

            var id = await service.AddPlayEventAsync(dto);

            Assert.Equal(1, id);
            Assert.Single(playEventRepo.Items);
            var stored = playEventRepo.Items.First();
            Assert.Equal(dto.UserID, stored.UserID);
            Assert.Equal(dto.TrackID, stored.TrackID);
            Assert.Equal(dto.SessionID, stored.SessionID);
            Assert.Equal(dto.PlayDurationMs, stored.PlayDurationMs);
            Assert.False(stored.WasSkipped);
            Assert.Equal(dto.Context, stored.Context);
        }

        [Fact]
        public async Task GetPlayEventAsync_ReturnsDto_WhenPlayEventExists()
        {
            var playEvent = new PlayEvent
            {
                PlayEventID = 99,
                UserID = Guid.NewGuid(),
                TrackID = 5,
                SessionID = 20,
                PlayedAt = DateTime.UtcNow,
                PlayDurationMs = 5000,
                WasSkipped = false,
                Context = "Playback"
            };
            var playEventRepo = new PlayEventServiceFakePlayEventRepository(new[] { playEvent });
            var unitOfWork = new PlayEventServiceFakeUnitOfWork { PlayEventRepository = playEventRepo };
            var service = new PlayEventService(unitOfWork);

            var result = await service.GetPlayEventAsync(99);

            Assert.NotNull(result);
            Assert.Equal(playEvent.PlayEventID, result!.PlayEventID);
            Assert.Equal(playEvent.TrackID, result.TrackID);
            Assert.Equal(playEvent.PlayDurationMs, result.PlayDurationMs);
        }

        [Fact]
        public async Task GetPlayEventAsync_ReturnsNull_WhenPlayEventDoesNotExist()
        {
            var unitOfWork = new PlayEventServiceFakeUnitOfWork { PlayEventRepository = new PlayEventServiceFakePlayEventRepository() };
            var service = new PlayEventService(unitOfWork);

            var result = await service.GetPlayEventAsync(1234);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPlayEventsForTrackAsync_ReturnsOnlyMatchingTrackEvents()
        {
            var userId = Guid.NewGuid();
            var events = new[]
            {
                new PlayEvent { PlayEventID = 1, UserID = userId, TrackID = 5, PlayedAt = DateTime.UtcNow, PlayDurationMs = 1000, WasSkipped = false },
                new PlayEvent { PlayEventID = 2, UserID = userId, TrackID = 6, PlayedAt = DateTime.UtcNow, PlayDurationMs = 2000, WasSkipped = false }
            };
            var playEventRepo = new PlayEventServiceFakePlayEventRepository(events);
            var unitOfWork = new PlayEventServiceFakeUnitOfWork { PlayEventRepository = playEventRepo };
            var service = new PlayEventService(unitOfWork);

            var result = await service.GetPlayEventsForTrackAsync(5);

            Assert.Single(result);
            Assert.Equal(5, result.First().TrackID);
        }

        [Fact]
        public async Task GetPlayEventsForUserAsync_ReturnsOnlyMatchingUserEvents()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var events = new[]
            {
                new PlayEvent { PlayEventID = 1, UserID = userId1, TrackID = 5, PlayedAt = DateTime.UtcNow, PlayDurationMs = 1000, WasSkipped = false },
                new PlayEvent { PlayEventID = 2, UserID = userId2, TrackID = 5, PlayedAt = DateTime.UtcNow, PlayDurationMs = 2000, WasSkipped = false }
            };
            var playEventRepo = new PlayEventServiceFakePlayEventRepository(events);
            var unitOfWork = new PlayEventServiceFakeUnitOfWork { PlayEventRepository = playEventRepo };
            var service = new PlayEventService(unitOfWork);

            var result = await service.GetPlayEventsForUserAsync(userId1);

            Assert.Single(result);
            Assert.Equal(userId1, result.First().UserID);
        }
    }

    internal class PlayEventServiceFakePlayEventRepository : IPlayEventRepository
    {
        private readonly List<PlayEvent> _data;
        public PlayEventServiceFakePlayEventRepository() => _data = new List<PlayEvent>();
        public PlayEventServiceFakePlayEventRepository(IEnumerable<PlayEvent> items) => _data = new List<PlayEvent>(items);
        public IEnumerable<PlayEvent> Items => _data;

        public Task AddAsync(PlayEvent entity)
        {
            if (entity.PlayEventID == 0)
            {
                entity.PlayEventID = _data.Count == 0 ? 1 : _data.Max(x => x.PlayEventID) + 1;
            }
            _data.Add(entity);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<PlayEvent>> GetAllAsync() => Task.FromResult<IEnumerable<PlayEvent>>(_data);

        public Task<PlayEvent?> GetByIdAsync(object id)
        {
            if (id is long longId)
            {
                return Task.FromResult(_data.FirstOrDefault(x => x.PlayEventID == longId));
            }
            return Task.FromResult<PlayEvent?>(null);
        }

        public Task<IEnumerable<PlayEvent>> GetByUserIdAsync(Guid userId) => Task.FromResult<IEnumerable<PlayEvent>>(_data.Where(x => x.UserID == userId));

        public Task<IEnumerable<PlayEvent>> GetByTrackIdAsync(long trackId) => Task.FromResult<IEnumerable<PlayEvent>>(_data.Where(x => x.TrackID == trackId));

        public void Remove(PlayEvent entity) => _data.Remove(entity);

        public void Update(PlayEvent entity)
        {
            // in-memory objects are updated by reference
        }
    }

    internal class PlayEventServiceFakeUnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepository { get; set; } = null!;
        public ITrackRepository TrackRepository { get; set; } = null!;
        public IArtistRepository ArtistRepository { get; set; } = null!;
        public IMoodTypeRepository MoodTypeRepository { get; set; } = null!;
        public IAudioFeatureRepository AudioFeatureRepository { get; set; } = null!;
        public IUserProfileRepository ProfileRepository { get; set; } = null!;
        public IMoodEntryRepository MoodEntryRepository { get; set; } = null!;
        public IMoodEntryPlayLinkRepository MoodEntryPlayLinkRepository { get; set; } = null!;
        public IPlayEventRepository PlayEventRepository { get; set; } = null!;
        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }
}
