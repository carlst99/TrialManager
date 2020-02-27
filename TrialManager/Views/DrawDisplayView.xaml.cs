using System.Text.RegularExpressions;
using System.Windows.Controls;
using TrialManager.Model;
using TrialManager.Services;

namespace TrialManager.Views
{
    /// <summary>
    /// Interaction logic for DataImportView.xaml
    /// </summary>
    public partial class DrawDisplayView : UserControl
    {
        private readonly ILocationService _locationService;

        public DrawDisplayView(ILocationService locationService)
        {
            InitializeComponent();
            _locationService = locationService;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AddressAutocompleter.AutocompleteSource = new LocationAutoCompleteSource(_locationService);
        }

        private void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Only allow number input
            if (!Regex.Match(e.Text, "[0-9]").Success)
                e.Handled = true;
        }

        private void TxtBxMaxDogs_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Only allow number input
            if (Regex.Match(e.Text, "[0-9]").Success)
            {
                if ((TxtBxMaxDogs.Text.Length == 0 || TxtBxMaxDogs.SelectionStart == 0) && e.Text == "0")
                    e.Handled = true;
            } else
            {
                e.Handled = true;
            }
        }
    }
}
