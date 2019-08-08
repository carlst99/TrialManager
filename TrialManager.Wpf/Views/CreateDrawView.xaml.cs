using MaterialDesignExtensions.Model;
using MvvmCross;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System.Collections;
using System.Text.RegularExpressions;
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

        private void TxtBxRunCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Regex.Match(e.Text, "[0-9]").Success)
                e.Handled = true;
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
