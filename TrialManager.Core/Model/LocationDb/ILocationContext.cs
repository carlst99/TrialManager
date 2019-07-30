using Microsoft.EntityFrameworkCore;

namespace TrialManager.Core.Model.LocationDb
{
    public interface ILocationContext
    {
        DbSet<TownCityLocation> TownsCities { get; set; }
        DbSet<SuburbLocalityLocation> SuburbsLocalities { get; set; }
    }
}
