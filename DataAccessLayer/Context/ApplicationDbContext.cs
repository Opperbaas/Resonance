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
        public DbSet<Song> Songs { get; set; }
    }
}