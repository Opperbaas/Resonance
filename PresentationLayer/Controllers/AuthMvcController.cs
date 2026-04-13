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
        private readonly IAuthService _authService;

        public AuthMvcController(ILogger<AuthMvcController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        [Route("/login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for {User}", dto?.Username);

            if (dto == null || !ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in both fields.";
                return View("Login", dto);
            }

            var result = await _authService.LoginAsync(dto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View("Login", dto);
            }

            HttpContext.Session.SetString("Username", dto.Username);
            HttpContext.Session.SetString("UserId", result.UserId?.ToString() ?? string.Empty);
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
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            _logger.LogInformation("Registration attempt for {User}", dto?.Username);

            if (dto == null || !ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all fields.";
                return View("Register", dto);
            }

            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View("Register", dto);
            }
            TempData["SuccessMessage"] = "Registration successful, please log in.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        [Route("/forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please enter your email address.";
                return View("ForgotPassword", dto);
            }

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/reset-password";
            dto.CallbackUrl = callbackUrl;
            var result = await _authService.RequestPasswordResetAsync(dto);

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("/reset-password")]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "Invalid password reset link.";
                return RedirectToAction("Login");
            }

            return View("ResetPassword", new ResetPasswordDto { Token = token });
        }

        [HttpPost]
        [Route("/reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all fields.";
                return View("ResetPassword", dto);
            }

            var result = await _authService.ResetPasswordAsync(dto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View("ResetPassword", dto);
            }

            TempData["SuccessMessage"] = result.Message;
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
