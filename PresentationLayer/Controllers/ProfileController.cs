using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Resonance.PresentationLayer.Controllers
{
    public class ProfileController : Controller
    {
        [HttpGet]
        [Route("/profile")]
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Login", "AuthMvc");
            }

            ViewBag.Username = user;
            return View();
        }
    }
}