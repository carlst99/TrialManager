﻿using MaterialDesignThemes.Wpf;
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
