using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resonance.DataAccessLayer.Models
{
    public class UserProfile
    {
        [Key]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public string PreferredLocale { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
        public PrivacyLevel PrivacyLevel { get; set; } = PrivacyLevel.Private;

        public void UpdatePreferences(string preferredLocale, string timeZone, PrivacyLevel privacyLevel)
        {
            PreferredLocale = preferredLocale;
            TimeZone = timeZone;
            PrivacyLevel = privacyLevel;
        }

        public User User { get; set; } = null!;
    }
}
