using MaterialDesignThemes.Wpf;
using Stylet;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TrialManager.Resources;
using TrialManager.Services;
using TrialManager.ViewModels.Base;
using TrialManager.Views;

namespace TrialManager.ViewModels
{
    public class ShellViewModel : ViewModelConductorBase
    {
        private readonly DrawDisplayViewModel _drawDisplayViewModel;
        private readonly DataImportViewModel _dataImportViewModel;
        private readonly AboutDialogViewModel _aboutDialogViewModel;
        private ISnackbarMessageQueue _messageQueue;

        public ISnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set => SetAndNotify(ref _messageQueue, value);
        }

        public ShellViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            DataImportViewModel dataImportViewModel,
            DrawDisplayViewModel drawDisplayViewModel,
            AboutDialogViewModel aboutDialogViewModel,
            ISnackbarMessageQueue messageQueue)
            : base (eventAggregator, navigationService)
        {
            _drawDisplayViewModel = drawDisplayViewModel;
            _dataImportViewModel = dataImportViewModel;
            _aboutDialogViewModel = aboutDialogViewModel;
            MessageQueue = messageQueue;
            ActiveItem = dataImportViewModel;
        }

        public static async Task OnDialogHostLoaded()
        {
            MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
            {
                Title = "Diagnostics Collection",
                Message = "TrialManager collects a small amount of data to help with diagnosing any issues the app encounters. This data includes: Device Name, Device OS, and data on any crashes that occur within the application. This data is completely anonymous and will never be shared with any parties besides the developer of the application. If you wish to, you can disable data collection using the buttons below. You can change your mind at any time by accessing the 'About' menu of TrialManager",
                CancelButtonContent = "Disable Diagnostics",
                OkayButtonContent = "Enable Diagnostics"
            });
            await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
        }

        public static void OnDocumentationRequested()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = HelpUrls.Default,
                UseShellExecute = true
            });
        }

        public async Task OnAboutRequested()
        {
            await DialogHost.Show(new AboutDialog(_aboutDialogViewModel), "MainDialogHost").ConfigureAwait(false);
        }

        protected override void OnNavigationRequested(object sender, Type e, object p)
        {
            if (e == _drawDisplayViewModel.GetType())
            {
                _drawDisplayViewModel.Prepare(p);
                ActiveItem = _drawDisplayViewModel;
            }
            else if (e == _dataImportViewModel.GetType())
            {
                ActiveItem = _dataImportViewModel;
            }
        }
    }
}
