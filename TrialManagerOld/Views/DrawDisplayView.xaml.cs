using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TrialManager.Services;
using TrialManager.Utils;

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

        private void NonZeroTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!(sender is TextBox text))
                throw new ArgumentException("Sender must be TextBox", nameof(sender));

            // Only allow number input
            if (Regex.Match(e.Text, "[0-9]").Success)
            {
                if ((text.Text.Length == 0 || text.SelectionStart == 0) && e.Text == "0")
                    e.Handled = true;
            } else
            {
                e.Handled = true;
            }
        }

        private void NonZeroTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(sender is TextBox text))
                throw new ArgumentException("Sender must be TextBox", nameof(sender));

            if (e.Key == System.Windows.Input.Key.Back)
            {
                if (text.Text.Length == 1 || text.SelectionStart == 0)
                    text.Text = "1";
            }
        }

        private void NonZeroTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is TextBox text))
                throw new ArgumentException("Sender must be TextBox", nameof(sender));

            if (string.IsNullOrEmpty(text.Text))
                text.Text = "1";
        }
    }
}
