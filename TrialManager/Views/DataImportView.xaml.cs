using MaterialDesignExtensions.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrialManager.Views
{
    /// <summary>
    /// Interaction logic for DataImportView.xaml
    /// </summary>
    public partial class DataImportView : UserControl
    {
        public DataImportView()
        {
            InitializeComponent();
        }

        private void ListView_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
            MouseWheelEventArgs e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent
            };
            ScrlVwrMain.RaiseEvent(e2);
        }
    }
}
