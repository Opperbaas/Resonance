using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.BusinessLogicLayer.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;

        public DashboardService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(Guid userId)
        {
            var playEvents = (await _uow.PlayEventRepository.GetByUserIdAsync(userId)).ToList();
            var moodEntries = (await _uow.MoodEntryRepository.GetByUserIdAsync(userId)).ToList();
            var links = (await _uow.MoodEntryPlayLinkRepository.GetAllAsync()).Where(link => moodEntries.Any(me => me.MoodEntryID == link.MoodEntryID) && playEvents.Any(pe => pe.PlayEventID == link.PlayEventID)).ToList();
            var tracks = (await _uow.TrackRepository.GetAllAsync()).ToList();
            var artists = (await _uow.ArtistRepository.GetAllAsync()).ToList();
            var moodTypes = (await _uow.MoodTypeRepository.GetAllActiveAsync()).ToList();

            var moodTypeMap = moodTypes.ToDictionary(mt => mt.MoodTypeID, mt => mt.Label);
            var trackMap = tracks.ToDictionary(t => t.TrackID, t => t);
            var artistMap = artists.ToDictionary(a => a.ArtistID, a => a.Name);

            var listenedTracks = links
                .Select(link =>
                {
                    var playEvent = playEvents.FirstOrDefault(pe => pe.PlayEventID == link.PlayEventID);
                    var moodEntry = moodEntries.FirstOrDefault(me => me.MoodEntryID == link.MoodEntryID);
                    if (playEvent == null || moodEntry == null)
                        return null;

                    var track = trackMap.TryGetValue(playEvent.TrackID, out var trackVal) ? trackVal : null;
                    var artistName = track != null && artistMap.TryGetValue(track.ArtistID, out var name) ? name : "Unknown artist";
                    return new ListenedTrackMoodDto
                    {
                        TrackTitle = track?.Title ?? "Unknown track",
                        ArtistName = artistName,
                        MoodLabel = moodTypeMap.TryGetValue(moodEntry.MoodTypeID, out var label) ? label : $"Mood {moodEntry.MoodTypeID}",
                        Note = moodEntry.Note,
                        RelationType = link.RelationType,
                        PlayedAt = playEvent.PlayedAt
                    };
                })
                .Where(dto => dto != null)
                .Cast<ListenedTrackMoodDto>()
                .OrderByDescending(dto => dto.PlayedAt)
                .ToList();

            var weeklyMoodStats = moodEntries
                .GroupBy(entry => new { Year = entry.OccurredAt.Year, Week = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(entry.OccurredAt, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) })
                .Select(group => new
                {
                    group.Key.Year,
                    group.Key.Week,
                    TopMood = group.GroupBy(e => e.MoodTypeID).Select(g => new { MoodTypeID = g.Key, Count = g.Count() }).OrderByDescending(g => g.Count).First()
                })
                .Select(item => new WeeklyMoodStatDto
                {
                    WeekLabel = $"W{item.Week} {item.Year}",
                    MoodLabel = moodTypeMap.TryGetValue(item.TopMood.MoodTypeID, out var label) ? label : $"Mood {item.TopMood.MoodTypeID}",
                    Count = item.TopMood.Count
                })
                .OrderBy(stat => stat.WeekLabel)
                .ToList();

            var moodTrackBreakdown = links
                .Select(link =>
                {
                    var moodEntry = moodEntries.FirstOrDefault(me => me.MoodEntryID == link.MoodEntryID);
                    var playEvent = playEvents.FirstOrDefault(pe => pe.PlayEventID == link.PlayEventID);
                    if (moodEntry == null || playEvent == null)
                        return null;
                    if (!trackMap.TryGetValue(playEvent.TrackID, out var track))
                        return null;

                    return new MoodTrackKey
                    {
                        MoodTypeID = moodEntry.MoodTypeID,
                        TrackTitle = track.Title
                    };
                })
                .Where(x => x != null)
                .Cast<MoodTrackKey>()
                .GroupBy(x => new { x.MoodTypeID, x.TrackTitle })
                .Select(group => new MoodTrackDto
                {
                    MoodLabel = moodTypeMap.TryGetValue(group.Key.MoodTypeID, out var label) ? label : $"Mood {group.Key.MoodTypeID}",
                    TrackTitle = group.Key.TrackTitle,
                    Count = group.Count()
                })
                .OrderByDescending(dto => dto.Count)
                .ToList();

            return new DashboardViewModel
            {
                ListenedTracks = listenedTracks,
                WeeklyMoodStats = weeklyMoodStats,
                MoodTrackBreakdown = moodTrackBreakdown,
                MoodTypes = moodTypes.Select(mt => new MoodTypeDto
                {
                    MoodTypeID = mt.MoodTypeID,
                    Label = mt.Label,
                    Emoji = mt.Emoji,
                    ColorHex = mt.ColorHex
                })
                .ToList()
            };
        }

        private class MoodTrackKey
        {
            public int MoodTypeID { get; set; }
            public string TrackTitle { get; set; } = string.Empty;
        }
    }
}
