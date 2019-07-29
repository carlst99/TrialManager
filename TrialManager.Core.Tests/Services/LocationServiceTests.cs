using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TrialManager.Core.Model.LocationDb;
using TrialManager.Core.Services;
using Xunit;

namespace TrialManager.Core.Tests.Services
{
    public class LocationServiceTests
    {
        [Fact]
        public void TestAutoComplete()
        {
            LocationService lService = new LocationService(GetLocationContext());
            List<string> suggestions = lService.GetAutoCompleteSuggestions("w");

            Assert.Equal(3, suggestions.Count);
        }

        private ILocationContext GetLocationContext()
        {
            using (LocationContext context = new LocationContext())
            {
                if (context.TownsCities.AnyAsync().Result)
                    return new LocationContext();

                context.TownsCities.Add(new TownCityLocation { Name = "Hamilton" });
                context.TownsCities.Add(new TownCityLocation { Name = "Whangarei" });
                context.TownsCities.Add(new TownCityLocation { Name = "Wanganui" });

                context.SuburbsLocalities.Add(new SuburbLocalityLocation { Name = "Waikato" });
                context.SuburbsLocalities.Add(new SuburbLocalityLocation { Name = "King Country" });
                context.SuburbsLocalities.Add(new SuburbLocalityLocation { Name = "Parnell" });

                context.SaveChanges();
            }
            return new LocationContext();
        }
    }
}
