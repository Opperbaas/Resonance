namespace Resonance.BusinessLogicLayer.DTOs
{
    public class MoodEntryPlayLinkDto
    {
        public long MoodEntryID { get; set; }
        public long PlayEventID { get; set; }
        public string? RelationType { get; set; }
        public int? WindowMinutes { get; set; }
    }
}
