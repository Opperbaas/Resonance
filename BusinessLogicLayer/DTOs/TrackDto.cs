using System;
namespace Resonance.BusinessLogicLayer.DTOs
{
    public class TrackDto
    {
        public string Provider { get; set; }
        public string ProviderTrackKey { get; set; }
        public string Title { get; set; }
        public long ArtistID { get; set; }
        public string Album { get; set; }
        public int DurationMs { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}