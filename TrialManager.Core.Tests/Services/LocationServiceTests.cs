using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TrialManager.Core.Model.LocationDb;
using TrialManager.Core.Services;
using Xunit;

namespace TrialManager.Core.Tests.Services
{
    public class LocationServiceTests
    {
        #region AutoComplete tests

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

        #endregion

        #region Resolver tests

        /// <summary>
        /// Tests that the resolver method correctly handles a non-existent location input
        /// </summary>
        [Fact]
        public void TestResolveInvalid()
        {
            LocationService lService = GetLocationService();
            bool result = lService.TryResolve("Rainbow's End", out LocationBase tLoc);

            Assert.False(result);
            Assert.Null(tLoc);
        }

        /// <summary>
        /// Tests that the resolver method returns the correct city, given valid inputs
        /// </summary>
        [Fact]
        public void TestResolveCity()
        {
            LocationService lService = GetLocationService();
            bool result = lService.TryResolve("Hamilton", out LocationBase tLoc);

            Assert.True(result);
            Assert.NotNull(tLoc);
            Assert.IsType<TownCityLocation>(tLoc);
            Assert.Equal("Hamilton", tLoc.Name);

            result = lService.TryResolve("32 Knightsmead Place, Hamilton, Waikato", out tLoc);
            Assert.True(result);
            Assert.NotNull(tLoc);
            Assert.IsType<TownCityLocation>(tLoc);
            Assert.Equal("Hamilton", tLoc.Name);
        }

        #endregion

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
