using System;

namespace Resonance.DataAccessLayer.Models
{
    public class PlayEvent
    {
        public long PlayEventID { get; set; }
        public Guid UserID { get; set; }
        public long TrackID { get; set; }
        public long? SessionID { get; set; }
        public DateTime PlayedAt { get; set; }
        public int? PlayDurationMs { get; set; }
        public bool WasSkipped { get; set; }
        public string? Context { get; set; }
    }
}
