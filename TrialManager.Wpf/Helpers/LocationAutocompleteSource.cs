using MaterialDesignExtensions.Model;
using MvvmCross;
using System.Collections;
using TrialManager.Core.Services;

namespace TrialManager.Wpf.Helpers
{
    public class LocationAutocompleteSource : IAutocompleteSource
    {
        private readonly ILocationService _locationService;

        public LocationAutocompleteSource()
        {
            _locationService = Mvx.IoCProvider.Resolve<ILocationService>();
        }

        public IEnumerable Search(string searchTerm)
        {
            return _locationService.GetAutoCompleteSuggestions(searchTerm);
        }
    }
}
