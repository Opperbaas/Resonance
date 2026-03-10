using System;

namespace Resonance.DataAccessLayer.Models
{
    public class Song
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Mood { get; set; }
        public Guid UserId { get; set; } // owner
    }
}