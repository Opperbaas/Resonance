using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Resonance.BusinessLogicLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace Resonance.PresentationLayer.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Route("/profile")]
        public async Task<IActionResult> Index()
        {
            var userIdValue = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            var profile = await _profileService.GetProfileAsync(userId);
            if (profile == null)
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            return View(profile);
        }
    }
}
