using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Resonance.PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(user))
            {
                ViewBag.Username = user;
            }
            return View();
        }
    }
}
