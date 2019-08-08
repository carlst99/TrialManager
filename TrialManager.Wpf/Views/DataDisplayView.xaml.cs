using MvvmCross.Platforms.Wpf.Views;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using TrialManager.Wpf.Helpers;

namespace TrialManager.Wpf.Views
{
    [DetailPresentation]
    public partial class DataDisplayView : MvxWpfView
    {
        public DataDisplayView()
        {
            InitializeComponent();
        }

        private void MvxWpfView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AddressAutocompleter.AutocompleteSource = new LocationAutocompleteSource();
        }
    }
}
