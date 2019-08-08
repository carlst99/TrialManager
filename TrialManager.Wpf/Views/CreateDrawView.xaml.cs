using MvvmCross.Platforms.Wpf.Views;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System.Text.RegularExpressions;
using TrialManager.Wpf.Helpers;

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
}
