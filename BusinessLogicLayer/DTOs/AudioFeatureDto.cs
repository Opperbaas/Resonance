using System;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class AudioFeatureDto
    {
        public long TrackID { get; set; }
        public float? Tempo { get; set; }
        public float? Energy { get; set; }
        public float? Valence { get; set; }
        public float? Danceability { get; set; }
        public float? Acousticness { get; set; }
        public float? Loudness { get; set; }
        public string? FeatureSource { get; set; }
        public DateTime? FeatureUpdatedAt { get; set; }
    }
}
