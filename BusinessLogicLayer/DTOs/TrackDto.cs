using System;
namespace Resonance.BusinessLogicLayer.DTOs
{
    public class TrackDto
    {
        public long TrackID { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderTrackKey { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public long ArtistID { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public int DurationMs { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PreviewUrl { get; set; } = string.Empty;
        public string SpotifyUrl { get; set; } = string.Empty;
    }
}