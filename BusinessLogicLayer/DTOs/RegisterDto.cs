namespace Resonance.BusinessLogicLayer.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}