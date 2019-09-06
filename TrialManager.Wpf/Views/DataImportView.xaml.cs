using MvvmCross.Platforms.Wpf.Views;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System.Text.RegularExpressions;
using TrialManager.Wpf.Helpers;

namespace TrialManager.Wpf.Views
{
    [DetailPresentation]
    public partial class DataImportView : MvxWpfView
    {
        public DataImportView()
        {
            InitializeComponent();
        }
    }
}
