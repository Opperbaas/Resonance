using System.ComponentModel.DataAnnotations;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords must match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
