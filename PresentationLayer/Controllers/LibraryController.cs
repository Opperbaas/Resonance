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
        private readonly IMoodEntryService _moodEntryService;
        private readonly IPlayEventService _playEventService;
        private readonly IMoodEntryPlayLinkService _moodEntryPlayLinkService;
        private readonly IUnitOfWork _uow;

        public LibraryController(
            ITrackService trackService,
            SpotifyApiClient spotifyApiClient,
            IMoodEntryService moodEntryService,
            IPlayEventService playEventService,
            IMoodEntryPlayLinkService moodEntryPlayLinkService,
            IUnitOfWork uow)
        {
            _trackService = trackService;
            _spotifyApiClient = spotifyApiClient;
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
            ViewBag.MoodTypes = await GetMoodTypesAsync();
            var tracks = await _trackService.GetTracksForUserAsync(userId);
            return View(tracks);
        }

        [HttpPost]
        [Route("/library/search")]
        public async Task<IActionResult> Search(string query)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return RedirectToAction("Login", "AuthMvc");

            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            ViewBag.Username = user;
            ViewBag.SearchQuery = query;
            ViewBag.SearchResults = await _spotifyApiClient.SearchTracksAsync(query);
            ViewBag.MoodTypes = await GetMoodTypesAsync();
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

            var moodEntryId = await _moodEntryService.AddMoodEntryAsync(new MoodEntryDto
            {
                UserID = userId,
                MoodTypeID = moodTypeId,
                OccurredAt = DateTime.UtcNow,
                Note = note,
                ContextTag = $"Track:{trackId}",
                Source = "TrackMood"
            });

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

            TempData["SuccessMessage"] = $"Mood toegevoegd aan '{track.Title}'.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/library/play")]
        public async Task<IActionResult> Play(string provider, string providerTrackKey)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return RedirectToAction("Login", "AuthMvc");

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerTrackKey))
                return RedirectToAction("Index");

            if (!provider.Equals("Spotify", StringComparison.OrdinalIgnoreCase))
                return View("Play");

            var (previewUrl, spotifyUrl) = await _spotifyApiClient.GetTrackPreviewAndUrlAsync(providerTrackKey);
            ViewBag.PreviewUrl = previewUrl;
            ViewBag.SpotifyUrl = spotifyUrl;
            ViewBag.TrackName = providerTrackKey;
            ViewBag.Provider = provider;
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
    }
}
