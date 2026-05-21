using System.Collections.Generic;

namespace Resonance.DataAccessLayer.Models
{
    public class Artist
    {
        public long ArtistID { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
