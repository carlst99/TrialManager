using Microsoft.EntityFrameworkCore;

namespace TrialManager.Core.Model.LocationDb
{
    public class LocationContext : DbContext, ILocationContext
    {
        public DbSet<TownCityLocation> TownsCities { get; set; }
        public DbSet<SuburbLocalityLocation> SuburbsLocalities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Resources\\locations.db");
        }
    }
}
