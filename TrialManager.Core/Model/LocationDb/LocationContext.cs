using MessagePack;
using Microsoft.EntityFrameworkCore;

namespace TrialManager.Core.Model.LocationDb
{
    public class LocationContext : DbContext, ILocationContext
    {
        public DbSet<TownCityLocation> TownsCities { get; set; }
        public DbSet<SuburbLocalityLocation> SuburbsLocalities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if TEST
            optionsBuilder.UseInMemoryDatabase("LocationsDatabase");
#else
            optionsBuilder.UseSqlite("Data Source=Resources\\locations.db");
#endif
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
