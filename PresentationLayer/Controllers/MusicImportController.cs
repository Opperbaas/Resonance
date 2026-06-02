using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.PresentationLayer.Controllers
{
    public class MusicImportController : Controller
    {
        private readonly IMusicImportService _musicImportService;

        public MusicImportController(IMusicImportService musicImportService)
        {
            _musicImportService = musicImportService;
        }

        [HttpGet]
        [Route("/music/import")]
        public IActionResult Index()
        {
            var providers = _musicImportService.GetAvailableProviders();
            return View(providers);
        }

        [HttpPost]
        [Route("/music/import/library")]
        public async Task<IActionResult> ImportLibrary(string provider = "Spotify")
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            if (string.IsNullOrWhiteSpace(provider))
            {
                TempData["ErrorMessage"] = "Please select a music provider.";
                return RedirectToAction("Index");
            }

            var importedTracks = await _musicImportService.ImportLibraryAsync(userId, provider);
            var importedCount = importedTracks?.Count() ?? 0;
            TempData["SuccessMessage"] = $"Imported {importedCount} track(s) from {provider}.";
            return RedirectToAction("Index", "Library");
        }

        [HttpPost]
        [Route("/music/import/sync")]
        public async Task<IActionResult> SyncRecentPlays(string provider = "Spotify")
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            if (string.IsNullOrWhiteSpace(provider))
            {
                TempData["ErrorMessage"] = "Please select a music provider.";
                return RedirectToAction("Index");
            }

            var syncSince = DateTime.UtcNow.AddDays(-7);
            var syncedPlays = await _musicImportService.SyncRecentPlaysAsync(userId, provider, syncSince);
            var syncedCount = syncedPlays?.Count() ?? 0;
            TempData["SuccessMessage"] = $"Synced {syncedCount} recent play(s) from {provider}.";
            return RedirectToAction("Index", "Library");
        }
    }
}
