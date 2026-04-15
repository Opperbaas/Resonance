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
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Track>().ToTable("Track");
            modelBuilder.Entity<UserProfile>().ToTable("UserProfile");
        }
    }
}