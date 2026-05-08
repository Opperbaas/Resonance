using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Context
{
    // EF Core DbContext: add DbSets for persistence entities
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<AudioFeature> AudioFeatures { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<MoodEntry> MoodEntries { get; set; }
        public DbSet<MoodEntryPlayLink> MoodEntryPlayLinks { get; set; }
        public DbSet<PlayEvent> PlayEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Track>().ToTable("Track");
            modelBuilder.Entity<AudioFeature>().ToTable("AudioFeature");
            modelBuilder.Entity<AudioFeature>().HasKey(feature => feature.TrackID);
            modelBuilder.Entity<AudioFeature>().HasOne(feature => feature.Track)
                .WithOne(track => track.AudioFeature)
                .HasForeignKey<AudioFeature>(feature => feature.TrackID);
            modelBuilder.Entity<UserProfile>().ToTable("UserProfile");
            modelBuilder.Entity<MoodEntry>().ToTable("MoodEntry");
            modelBuilder.Entity<MoodEntryPlayLink>().ToTable("MoodEntryPlayLink");
            modelBuilder.Entity<MoodEntryPlayLink>().HasKey(link => new { link.MoodEntryID, link.PlayEventID });
            modelBuilder.Entity<PlayEvent>().ToTable("PlayEvent");
        }
    }
}