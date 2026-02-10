namespace Resonance.BusinessLogicLayer.DTOs
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public System.Guid? UserId { get; set; }
    }
}