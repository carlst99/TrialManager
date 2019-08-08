using MaterialDesignExtensions.Model;
using MvvmCross;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System.Collections;
using TrialManager.Core.Services;

namespace TrialManager.Wpf.Views
{
    [DetailPresentation]
    public partial class CreateDrawView : MvxWpfView
    {
        public CreateDrawView()
        {
            InitializeComponent();
        }

        private void MvxWpfView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AddressAutocompleter.AutocompleteSource = new LocationAutocompleteSource();
        }
    }

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
