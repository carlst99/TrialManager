using System.Diagnostics;
using System.Windows.Controls;
using TrialManager.Resources;

namespace TrialManager.Views
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        private string _helpUrl;

        public string Title { get; set; }
        public string Message { get; set; }
        public string OkayButtonContent { get; set; }
        public string CancelButtonContent { get; set; }
        public string HelpUrl
        {
            get => _helpUrl;
            set => _helpUrl = value.Replace("&", "^&");
        }

        public MessageDialog()
        {
            InitializeComponent();
            DataContext = this;

            Title = "Alert";
            OkayButtonContent = "Okay";
            CancelButtonContent = null;
            HelpUrl = HelpUrls.Default;
        }

        public void OpenHelpUrl()
        {
            if (HelpUrl != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = HelpUrl,
                    UseShellExecute = true
                });
            }
        }
    }
}
