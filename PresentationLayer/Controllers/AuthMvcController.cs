using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Resonance.PresentationLayer.Controllers
{
    public class AuthMvcController : Controller
    {
        private readonly ILogger<AuthMvcController> _logger;

        public AuthMvcController(ILogger<AuthMvcController> logger)
        {
            _logger = logger;
        }

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
            _logger.LogInformation("Login attempt for {User}", dto?.Username);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in both fields.";
                return View("Login", dto);
            }

            var result = await authService.LoginAsync(dto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View("Login", dto);
            }

            HttpContext.Session.SetString("Username", dto.Username);
            HttpContext.Session.SetString("UserId", result.UserId.ToString());
            TempData["SuccessMessage"] = "You are now logged in!";

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
            _logger.LogInformation("Registration attempt for {User}", dto?.Username);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in both fields.";
                return View("Register", dto);
            }

            var result = await authService.RegisterAsync(dto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View("Register", dto);
            }
            TempData["SuccessMessage"] = "Registration successful, please log in.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
