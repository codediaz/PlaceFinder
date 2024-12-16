using Microsoft.EntityFrameworkCore;
using PlaceFinder.Models;

namespace PlaceFinder.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        { }

        // DbSets 
        public DbSet<User> Users { get; set; }
        public DbSet<SavedPlace> SavedPlaces { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Place>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Suggestion>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<SavedPlace>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.SavedPlaces)
                .HasForeignKey(sp => sp.UserId);

            modelBuilder.Entity<SavedPlace>()
                .HasOne(sp => sp.Place)
                .WithMany(p => p.SavedPlaces)
                .HasForeignKey(sp => sp.PlaceId);

            modelBuilder.Entity<Suggestion>()
                .HasOne(s => s.Place)
                .WithMany(p => p.Suggestions)
                .HasForeignKey(s => s.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Suggestion>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
