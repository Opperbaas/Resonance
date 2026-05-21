using System;
using Resonance.DataAccessLayer.Models;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class ProfileDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PreferredLocale { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
        public PrivacyLevel PrivacyLevel { get; set; }
    }
}
