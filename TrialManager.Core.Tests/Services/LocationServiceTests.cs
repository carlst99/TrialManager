using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TrialManager.Core.Model.LocationDb;
using TrialManager.Core.Services;
using Xunit;

namespace TrialManager.Core.Tests.Services
{
    public class LocationServiceTests
    {
        /// <summary>
        /// This tests that the autocomplete method returns the correct amount
        /// </summary>
        [Fact]
        public void TestAutoCompleteReturnCount()
        {
            LocationService lService = GetLocationService();
            List<string> suggestions = lService.GetAutoCompleteSuggestions("w");
            Assert.Equal(3, suggestions.Count);
        }

        /// <summary>
        /// This tests that the autocomplete method ignores case
        /// </summary>
        [Fact]
        public void TestAutoCompleteInputCase()
        {
            LocationService lService = GetLocationService();
            List<string> suggestions = lService.GetAutoCompleteSuggestions("h");
            Assert.Single(suggestions);

            suggestions = lService.GetAutoCompleteSuggestions("H");
            Assert.Single(suggestions);
        }

        /// <summary>
        /// This tests that the autocomplete method returns the correct suggestions
        /// </summary>
        [Fact]
        public void TestAutoCompleteReturn()
        {
            LocationService lService = GetLocationService();
            List<string> suggestions = lService.GetAutoCompleteSuggestions("k");
            Assert.Equal("King Country", suggestions[0]);

            suggestions = lService.GetAutoCompleteSuggestions("hamil");
            Assert.Equal("Hamilton", suggestions[0]);

            suggestions = lService.GetAutoCompleteSuggestions("Parnell, A");
            Assert.Equal("Parnell, Auckland", suggestions[0]);
        }

        private LocationService GetLocationService()
        {
            using (LocationContext context = new LocationContext())
            {
                if (context.TownsCities.AnyAsync().Result)
                    return new LocationService(new LocationContext());

                context.TownsCities.Add(new TownCityLocation { Name = "Hamilton" });
                context.TownsCities.Add(new TownCityLocation { Name = "Whangarei" });
                context.TownsCities.Add(new TownCityLocation { Name = "Wanganui" });

                context.SuburbsLocalities.Add(new SuburbLocalityLocation { Name = "Waikato" });
                context.SuburbsLocalities.Add(new SuburbLocalityLocation { Name = "King Country" });
                context.SuburbsLocalities.Add(new SuburbLocalityLocation { Name = "Parnell", TownCityName = "Auckland" });

                context.SaveChanges();
            }
            return new LocationService(new LocationContext());
        }
    }
}
