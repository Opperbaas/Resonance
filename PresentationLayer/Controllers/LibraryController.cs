using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace Resonance.PresentationLayer.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ISongService _songService;

        public LibraryController(ISongService songService)
        {
            _songService = songService;
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
            var songs = await _songService.GetSongsForUserAsync(userId);
            return View(songs);
        }

        [HttpPost]
        [Route("/library/add")]
        public async Task<IActionResult> Add(SongDto dto)
        {
            var user = HttpContext.Session.GetString("Username");
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return RedirectToAction("Login", "AuthMvc");

            await _songService.AddSongAsync(dto, userId);
            return RedirectToAction("Index");
        }
    }
}
