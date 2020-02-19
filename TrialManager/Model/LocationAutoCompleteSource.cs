using MaterialDesignExtensions.Model;
using System.Collections;
using TrialManager.Services;

namespace TrialManager.Model
{
    public class LocationAutoCompleteSource : IAutocompleteSource
    {
        private readonly ILocationService _locationService;

        public LocationAutoCompleteSource(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public IEnumerable Search(string searchTerm)
        {
            return _locationService.GetAutoCompleteSuggestions(searchTerm);
        }
    }
}
