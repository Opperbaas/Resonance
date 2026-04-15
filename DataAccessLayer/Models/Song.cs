using System;

namespace Resonance.DataAccessLayer.Models
{
    public class Track
    {
        public long TrackID { get; set; }
        public string Provider { get; set; } = null!;
        public string ProviderTrackKey { get; set; } = null!;
        public string Title { get; set; } = null!;
        public long ArtistID { get; set; }
        public string Album { get; set; } = null!;
        public int DurationMs { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}