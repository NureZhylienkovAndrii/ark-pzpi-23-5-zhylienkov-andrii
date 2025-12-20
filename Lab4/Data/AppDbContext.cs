namespace AutoRent.Data
{
    using Microsoft.EntityFrameworkCore;
    using AutoRent.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .Property(c => c.PricePerDay)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarStatus> CarStatus { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
