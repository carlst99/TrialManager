using MessagePack;
using Microsoft.EntityFrameworkCore;

namespace LINZCsvConverter.Model
{
    public class LocationContext : DbContext
    {
        public DbSet<TownCityLocation> TownsCities { get; set; }
        public DbSet<SuburbLocalityLocation> SuburbsLocalities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=locations.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationBase>()
                        .Property(l => l.Location)
                        .HasConversion(
                            f => MessagePackSerializer.Serialize(f),
                            b => MessagePackSerializer.Deserialize<Location>(b));
            base.OnModelCreating(modelBuilder);
        }
    }
}
