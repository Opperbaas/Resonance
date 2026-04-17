using System;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class MoodEntryDto
    {
        public long MoodEntryID { get; set; }
        public Guid UserID { get; set; }
        public int MoodTypeID { get; set; }
        public DateTime OccurredAt { get; set; }
        public string? Note { get; set; }
        public string? ContextTag { get; set; }
        public string? Source { get; set; }
    }
}
