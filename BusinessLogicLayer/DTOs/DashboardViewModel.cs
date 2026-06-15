using System;
using System.Collections.Generic;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class DashboardViewModel
    {
        public string MostPlayedTrack { get; set; } = string.Empty;
        public string MostCommonMood { get; set; } = string.Empty;
        public long TotalPlaybackDurationMs { get; set; }
        public int TotalPlaybackEvents { get; set; }
        public double AveragePlaybackDurationMs { get; set; }
        public string MostPlayedTrackByDuration { get; set; } = string.Empty;
        public IEnumerable<MoodDistributionDto> MoodDistribution { get; set; } = new List<MoodDistributionDto>();
        public IEnumerable<ListenedTrackMoodDto> ListenedTracks { get; set; } = new List<ListenedTrackMoodDto>();
        public IEnumerable<WeeklyMoodStatDto> WeeklyMoodStats { get; set; } = new List<WeeklyMoodStatDto>();
        public IEnumerable<MoodTrackDto> MoodTrackBreakdown { get; set; } = new List<MoodTrackDto>();
        public IEnumerable<MoodTypeDto> MoodTypes { get; set; } = new List<MoodTypeDto>();
    }

    public class ListenedTrackMoodDto
    {
        public string TrackTitle { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string MoodLabel { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? RelationType { get; set; }
        public DateTime PlayedAt { get; set; }
    }

    public class WeeklyMoodStatDto
    {
        public string WeekLabel { get; set; } = string.Empty;
        public string MoodLabel { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MoodTrackDto
    {
        public string MoodLabel { get; set; } = string.Empty;
        public string TrackTitle { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MoodDistributionDto
    {
        public string MoodLabel { get; set; } = string.Empty;
        public int Count { get; set; }
        public string? ColorHex { get; set; }
    }

    public class MoodTypeDto
    {
        public int MoodTypeID { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? Emoji { get; set; }
        public string? ColorHex { get; set; }
    }
}
