using System;

namespace Resonance.DataAccessLayer.Models
{
    public class ItemEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}