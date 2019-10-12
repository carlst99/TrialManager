using MvvmCross.Platforms.Wpf.Views;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System;
using System.Windows.Data;
using TrialManager.Core.Model.TrialistDb;
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

        private void TxtBxSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (CollectionViewSource.GetDefaultView(LstVwTrialists.ItemsSource).Filter != TrialistFilter)
                CollectionViewSource.GetDefaultView(LstVwTrialists.ItemsSource).Filter = TrialistFilter;
            CollectionViewSource.GetDefaultView(LstVwTrialists.ItemsSource).Refresh();
        }

        private bool TrialistFilter(object item)
        {
            if (string.IsNullOrEmpty(TxtBxSearch.Text))
                return true;

            var triailst = (Trialist)item;

            return triailst.Name.StartsWith(TxtBxSearch.Text, StringComparison.OrdinalIgnoreCase);
        }
    }
}
