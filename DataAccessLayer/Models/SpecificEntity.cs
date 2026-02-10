using System;

namespace Resonance.DataAccessLayer.Models
{
    public class SpecificEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}