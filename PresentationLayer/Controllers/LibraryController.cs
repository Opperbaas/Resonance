using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.BusinessLogicLayer.Integrations;
using Resonance.DataAccessLayer.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Resonance.PresentationLayer.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly SpotifyApiClient _spotifyApiClient;
        private readonly YouTubeApiClient _youTubeApiClient;
        private readonly IMoodEntryService _moodEntryService;
        private readonly IPlayEventService _playEventService;
        private readonly IMoodEntryPlayLinkService _moodEntryPlayLinkService;
        private readonly IUnitOfWork _uow;

        public LibraryController(
            ITrackService trackService,
            SpotifyApiClient spotifyApiClient,
            YouTubeApiClient youTubeApiClient,
            IMoodEntryService moodEntryService,
            IPlayEventService playEventService,
            IMoodEntryPlayLinkService moodEntryPlayLinkService,
            IUnitOfWork uow)
        {
            _trackService = trackService;
            _spotifyApiClient = spotifyApiClient;
            _youTubeApiClient = youTubeApiClient;
            _moodEntryService = moodEntryService;
            _playEventService = playEventService;
            _moodEntryPlayLinkService = moodEntryPlayLinkService;
            _uow = uow;
        }

        [HttpGet]
        [Route("/library")]
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            ViewBag.Username = user;
            ViewBag.SpotifyConnected = !string.IsNullOrEmpty(HttpContext.Session.GetString("SpotifyAccessToken"));
            ViewBag.MoodTypes = await GetMoodTypesAsync();
            ViewBag.TrackMoodMap = await GetTrackMoodMapAsync(userId);
            var tracks = await _trackService.GetTracksForUserAsync(userId);
            return View(tracks);
        }

        [HttpPost]
        [Route("/library/search")]
        public async Task<IActionResult> Search(string query, string provider = "Spotify")
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return RedirectToAction("Login", "AuthMvc");

            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            ViewBag.Username = user;
            ViewBag.SpotifyConnected = !string.IsNullOrEmpty(HttpContext.Session.GetString("SpotifyAccessToken"));
            ViewBag.SearchQuery = query;
            ViewBag.SearchProvider = provider;
            ViewBag.MoodTypes = await GetMoodTypesAsync();
            ViewBag.TrackMoodMap = await GetTrackMoodMapAsync(userId);

            if (provider.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.SearchResults = await _youTubeApiClient.SearchVideosAsync(query);
            }
            else
            {
                ViewBag.SearchResults = await _spotifyApiClient.SearchTracksAsync(query);
            }

            var tracks = await _trackService.GetTracksForUserAsync(userId);
            return View("Index", tracks);
        }

        [HttpPost]
        [Route("/library/add")]
        public async Task<IActionResult> Add(TrackDto dto)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return RedirectToAction("Login", "AuthMvc");

            await _trackService.AddTrackAsync(dto, userId);
            TempData["SuccessMessage"] = $"Added '{dto.Title}' to your library.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/library/delete")]
        public async Task<IActionResult> Delete(long trackId)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return RedirectToAction("Login", "AuthMvc");

            await _trackService.DeleteTrackAsync(trackId);
            TempData["SuccessMessage"] = "Track verwijderd uit je library.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/library/mood")]
        public async Task<IActionResult> AddMood(long trackId, int moodTypeId, string? note)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return RedirectToAction("Login", "AuthMvc");

            var track = await _uow.TrackRepository.GetByIdAsync(trackId);
            if (track == null)
                return RedirectToAction("Index");

            var contextTag = $"Track:{trackId}";
            var existingMood = await _moodEntryService.GetLatestMoodEntryForTrackAsync(userId, trackId);
            long moodEntryId;

            if (existingMood != null)
            {
                existingMood.MoodTypeID = moodTypeId;
                existingMood.Note = note;
                existingMood.OccurredAt = DateTime.UtcNow;
                existingMood.ContextTag = contextTag;
                existingMood.Source = "TrackMood";

                await _moodEntryService.UpdateMoodEntryAsync(existingMood);
                moodEntryId = existingMood.MoodEntryID;
            }
            else
            {
                moodEntryId = await _moodEntryService.AddMoodEntryAsync(new MoodEntryDto
                {
                    UserID = userId,
                    MoodTypeID = moodTypeId,
                    OccurredAt = DateTime.UtcNow,
                    Note = note,
                    ContextTag = contextTag,
                    Source = "TrackMood"
                });
            }

            var playEventId = await _playEventService.AddPlayEventAsync(new PlayEventDto
            {
                UserID = userId,
                TrackID = trackId,
                PlayedAt = DateTime.UtcNow,
                WasSkipped = false,
                Context = "MoodTag"
            });

            await _moodEntryPlayLinkService.AddLinkAsync(new MoodEntryPlayLinkDto
            {
                MoodEntryID = moodEntryId,
                PlayEventID = playEventId,
                RelationType = "MoodTag",
                WindowMinutes = 5
            });

            TempData["SuccessMessage"] = $"Mood bijgewerkt voor '{track.Title}'.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/library/playevent")]
        public async Task<IActionResult> AddPlayEvent([FromBody] PlayEventDto dto)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            if (dto == null || dto.TrackID <= 0)
                return BadRequest("Invalid track information.");

            await _playEventService.AddPlayEventAsync(new PlayEventDto
            {
                UserID = userId,
                TrackID = dto.TrackID,
                SessionID = dto.SessionID,
                PlayedAt = DateTime.UtcNow,
                PlayDurationMs = dto.PlayDurationMs,
                WasSkipped = dto.WasSkipped,
                Context = dto.Context ?? "Playback"
            });

            return Ok();
        }

        [HttpGet]
        [Route("/library/play")]
        public async Task<IActionResult> Play(string provider, string providerTrackKey, long? trackId)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return RedirectToAction("Login", "AuthMvc");

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerTrackKey))
                return RedirectToAction("Index");

            ViewBag.TrackName = providerTrackKey;
            ViewBag.Provider = provider;
            ViewBag.ProviderTrackKey = providerTrackKey;
            ViewBag.TrackId = trackId ?? 0;

            if (provider.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.PreviewUrl = _youTubeApiClient.GetEmbedUrl(providerTrackKey);
                ViewBag.ProviderUrl = _youTubeApiClient.GetWatchUrl(providerTrackKey);
                return View("Play");
            }

            var (previewUrl, spotifyUrl) = await _spotifyApiClient.GetTrackPreviewAndUrlAsync(providerTrackKey);
            ViewBag.PreviewUrl = previewUrl;
            ViewBag.SpotifyUrl = spotifyUrl;
            ViewBag.SpotifyTrackKey = providerTrackKey;
            return View("Play");
        }

        private async Task<IEnumerable<MoodTypeDto>> GetMoodTypesAsync()
        {
            var moodTypes = await _uow.MoodTypeRepository.GetAllActiveAsync();
            return moodTypes.Select(mt => new MoodTypeDto
            {
                MoodTypeID = mt.MoodTypeID,
                Label = mt.Label,
                Emoji = mt.Emoji,
                ColorHex = mt.ColorHex
            });
        }

        private async Task<Dictionary<long, MoodEntryDto>> GetTrackMoodMapAsync(Guid userId)
        {
            var entries = await _moodEntryService.GetMoodEntriesForUserAsync(userId);
            return entries
                .Select(entry => new { TrackId = ExtractTrackIdFromContext(entry.ContextTag), Entry = entry })
                .Where(x => x.TrackId.HasValue)
                .GroupBy(x => x.TrackId!.Value)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Entry.OccurredAt).First().Entry);
        }

        private long? ExtractTrackIdFromContext(string? contextTag)
        {
            if (string.IsNullOrWhiteSpace(contextTag))
                return null;

            const string prefix = "Track:";
            if (!contextTag.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return null;

            if (long.TryParse(contextTag.Substring(prefix.Length), out var trackId))
                return trackId;

            return null;
        }
    }
}
