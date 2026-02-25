using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using System.Threading.Tasks;

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

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(LoginDto dto, [FromServices] IAuthService authService)
        {
            if (!ModelState.IsValid)
                return View("Login", dto);

            var result = await authService.LoginAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("Login", dto);
            }

            // TODO: store token in cookie/session if needed
            // For now just redirect to home page on success
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/register")]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(RegisterDto dto, [FromServices] IAuthService authService)
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