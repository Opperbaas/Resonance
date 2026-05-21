using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.PresentationLayer.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("/dashboard")]
        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var user = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            ViewBag.Username = user;
            var model = await _dashboardService.GetDashboardAsync(userId);
            return View(model);
        }
    }
}
