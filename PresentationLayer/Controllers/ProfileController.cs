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

        [HttpPost]
        [Route("/profile/update-username")]
        public async Task<IActionResult> UpdateUsername(string username)
        {
            var userIdValue = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            username = username?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["ErrorMessage"] = "Username cannot be empty.";
                return RedirectToAction("Index");
            }

            if (await _profileService.IsUsernameTakenAsync(username, userId))
            {
                TempData["ErrorMessage"] = "That username is already taken. Please choose another.";
                return RedirectToAction("Index");
            }

            var success = await _profileService.UpdateUsernameAsync(userId, username);
            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to update username. Please try again.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Username has been updated.";
            return RedirectToAction("Index");
        }
    }
}
