namespace Resonance.DataAccessLayer.Models
{
    public class MoodType
    {
        public int MoodTypeID { get; set; }
        public string Label { get; set; } = null!;
        public string? Emoji { get; set; }
        public string? ColorHex { get; set; }
        public bool IsActive { get; set; }
    }
}
