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
        private readonly ITrackService _trackService;

        public LibraryController(ITrackService trackService)
        {
            _trackService = trackService;
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
            var tracks = await _trackService.GetTracksForUserAsync(userId);
            return View(tracks);
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
            return RedirectToAction("Index");
        }
    }
}
