using MaterialDesignThemes.Wpf;
using Stylet;
using TrialManager.Services;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public class ShellViewModel : ViewModelConductorBase
    {
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
            ISnackbarMessageQueue messageQueue)
            : base (eventAggregator, navigationService)
        {
            MessageQueue = messageQueue;
            ActiveItem = dataImportViewModel;
        }

        public void OnDocumentationRequested()
        {
            MessageQueue.Enqueue("Help documentation has not yet been added. Please wait for an upcoming update!");
        }
    }
}
