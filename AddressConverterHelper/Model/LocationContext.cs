using Microsoft.EntityFrameworkCore;

namespace AddressConverterHelper.Model
{
    public class LocationContext : DbContext
    {
        public DbSet<TownCityLocation> TownsCities { get; set; }
        public DbSet<SuburbLocalityLocation> SuburbsLocalities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=locations.db");
        }
    }
}
