namespace Resonance.BusinessLogicLayer.DTOs
{
    public class RegisterResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public System.Guid? UserId { get; set; }
    }
}