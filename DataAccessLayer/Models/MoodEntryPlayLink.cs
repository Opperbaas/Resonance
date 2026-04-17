using System;

namespace Resonance.DataAccessLayer.Models
{
    public class MoodEntryPlayLink
    {
        public long MoodEntryID { get; set; }
        public long PlayEventID { get; set; }
        public string? RelationType { get; set; }
        public int? WindowMinutes { get; set; }
    }
}
