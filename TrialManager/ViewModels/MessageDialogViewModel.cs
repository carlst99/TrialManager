using TrialManager.Resources;

namespace TrialManager.ViewModels
{
    public class MessageDialogViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string OkayButtonContent { get; set; }
        public string CancelButtonContent { get; set; }
        public string HelpUrl { get; set; }

        public MessageDialogViewModel()
        {
            Title = "Alert";
            OkayButtonContent = "Okay";
            CancelButtonContent = null;
            HelpUrl = HelpUrls.Default;
        }
    }
}
