using MvvmCross.Platforms.Wpf.Views;
using TrialManager.Wpf.Helpers;

namespace TrialManager.Wpf.Views
{
    public partial class HomeView : MvxWpfView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void MvxWpfView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //AddressAutocompleter1.AutocompleteSource = new LocationAutocompleteSource();
            //AddressAutocompleter2.AutocompleteSource = new LocationAutocompleteSource();
        }
    }
}
