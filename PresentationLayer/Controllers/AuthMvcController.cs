using Microsoft.AspNetCore.Mvc;

namespace Resonance.PresentationLayer.Controllers
{
    public class AuthMvcController : Controller
    {
        [HttpGet]
        [Route("/login")]
        public IActionResult Login()
        {
            return View("Login");
        }
    }
}