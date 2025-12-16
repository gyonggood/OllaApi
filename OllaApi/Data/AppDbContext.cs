using Microsoft.EntityFrameworkCore;
using OllaApi.Models;
using OllaApi.Models.OllaApi.Models;

namespace OllaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<AdPhoto> AdPhotos { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<AdView> AdViews { get; set; }
        public DbSet<UserBan> UserBans { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.AdId });
        }
    }
}