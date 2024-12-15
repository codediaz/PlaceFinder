using Microsoft.EntityFrameworkCore;
using PlaceFinder.Models;

namespace PlaceFinder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Place> Places { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<SavedPlace> SavedPlaces { get; set; }
    }
}
