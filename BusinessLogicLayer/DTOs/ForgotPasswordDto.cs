using System.ComponentModel.DataAnnotations;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string CallbackUrl { get; set; } = string.Empty;
    }
}
