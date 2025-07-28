using Microsoft.EntityFrameworkCore;

namespace Tatilse.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Hotel> Hotels => Set<Hotel>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Room> Rooms => Set<Room>();

        public DbSet<Feature> Features => Set<Feature>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.features)
                .WithMany(f => f.Hotels)
                .UsingEntity(j => j.ToTable("HotelFeatures"));
        }
    }
}
