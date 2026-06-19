namespace Resonance.BusinessLogicLayer.DTOs
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public System.Guid? UserId { get; set; }
    }
}