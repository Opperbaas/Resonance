using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.DTOs;

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

        [HttpGet]
        [Route("/register")]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(Resonance.BusinessLogicLayer.DTOs.RegisterDto dto, [FromServices] Resonance.BusinessLogicLayer.Interfaces.IAuthService authService)
        {
            if (!ModelState.IsValid)
                return View("Register", dto);

            var result = await authService.RegisterAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Register", dto);
            }
            // Redirect to login after successful registration
            return RedirectToAction("Login");
        }
    }
}