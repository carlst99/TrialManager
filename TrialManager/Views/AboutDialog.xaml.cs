using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using TrialManager.ViewModels;

namespace TrialManager.Views
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : UserControl
    {
        public AboutDialogViewModel ViewModel { get; set; }

        public AboutDialog(AboutDialogViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
            DataContext = ViewModel;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            });
        }

        private void Hyperlink_RequestFileNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.GetFullPath(e.Uri.ToString()),
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not open file");
            }
        }
    }
}
