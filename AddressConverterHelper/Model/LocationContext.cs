using Microsoft.EntityFrameworkCore;

namespace AddressConverterHelper.Model
{
    public class LocationContext : DbContext
    {
        public DbSet<TownCityLocation> TownCities { get; set; }
        public DbSet<SuburbLocalityLocation> SuburbLocalities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=locations.db");
        }
    }
}
