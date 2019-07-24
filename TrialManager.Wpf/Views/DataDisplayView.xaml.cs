using MvvmCross.Platforms.Wpf.Views;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;

namespace TrialManager.Wpf.Views
{
    [DetailPresentation]
    public partial class DataDisplayView : MvxWpfView
    {
        public DataDisplayView()
        {
            InitializeComponent();
        }
    }
}
