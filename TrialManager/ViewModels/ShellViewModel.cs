using MaterialDesignThemes.Wpf;
using Stylet;
using System;
using System.Diagnostics;
using TrialManager.Resources;
using TrialManager.Services;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public class ShellViewModel : ViewModelConductorBase
    {
        private ISnackbarMessageQueue _messageQueue;
        private DrawDisplayViewModel _drawDisplayViewModel;

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
            ISnackbarMessageQueue messageQueue)
            : base (eventAggregator, navigationService)
        {
            MessageQueue = messageQueue;
            _drawDisplayViewModel = drawDisplayViewModel;
            ActiveItem = dataImportViewModel;
        }

        public static void OnDocumentationRequested()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = HelpUrls.Default,
                UseShellExecute = true
            });
        }

        protected override void OnNavigationRequested(object sender, Type e, object p)
        {
            if (e == _drawDisplayViewModel.GetType())
            {
                _drawDisplayViewModel.Prepare(p);
                ActiveItem = _drawDisplayViewModel;
            }
        }
    }
}
