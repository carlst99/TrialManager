using System;
using System.Diagnostics;
using System.Windows.Controls;
using TrialManager.Resources;
using TrialManager.ViewModels;

namespace TrialManager.Views
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string OkayButtonContent { get; set; }
        public string CancelButtonContent { get; set; }
        public string HelpUrl { get; set; }

        public MessageDialog()
        {
            InitializeComponent();
            DataContext = this;

            Title = "Alert";
            OkayButtonContent = "Okay";
            CancelButtonContent = null;
            HelpUrl = HelpUrls.Default;
        }

        public MessageDialog(MessageDialogViewModel vm)
            : this()
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));

            Title = vm.Title;
            Message = vm.Message;
            OkayButtonContent = vm.OkayButtonContent;
            CancelButtonContent = vm.CancelButtonContent;
            HelpUrl = vm.HelpUrl;
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
