using MaterialDesignThemes.Wpf;
using Serilog;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model;
using TrialManager.Model.TrialistDb;
using TrialManager.Services;
using TrialManager.ViewModels.Base;
using TrialManager.Views;

namespace TrialManager.ViewModels
{
    public class DrawDisplayViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDrawCreationService _drawService;
        private readonly ISnackbarMessageQueue _messageQueue;

        private List<Trialist> _trialists;
        private List<TrialistDrawEntry> _draw;
        private string _trialAddress;
        private int _runsPerDay;
        private bool _showProgress;

        #endregion

        #region Properties

        public List<TrialistDrawEntry> Draw
        {
            get => _draw;
            set => SetAndNotify(ref _draw, value);
        }

        public string TrialAddress
        {
            get => _trialAddress;
            set => SetAndNotify(ref _trialAddress, value);
        }

        public int RunsPerDay
        {
            get => _runsPerDay;
            set => SetAndNotify(ref _runsPerDay, value);
        }

        public bool ShowProgress
        {
            get => _showProgress;
            set => SetAndNotify(ref _showProgress, value);
        }

        #endregion

        public DrawDisplayViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            IDrawCreationService drawService,
            ISnackbarMessageQueue messageQueue)
            : base(eventAggregator, navigationService)
        {
            _drawService = drawService;
            _messageQueue = messageQueue;
            _runsPerDay = 100;
        }

        public async Task CreateDraw()
        {
            ShowProgress = true;
            try
            {
                Draw = await Task.Run(() => _drawService.CreateDraw(_trialists, RunsPerDay, TrialAddress).ToList()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not create draw");
                MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
                {
                    Title = "This is embarrassing...",
                    Message = "TrialManager could not create the draw! Please try again"
                });
                await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
            }
            ShowProgress = false;
        }

        public async Task ImportNewData()
        {
            MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
            {
                Message = "Importing new data will mean you lose this current draw. Are you sure you want to continue?",
                OkayButtonContent = "Yes",
                CancelButtonContent = "No"
            });
            if ((bool)await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false))
            {
                Draw = null;
                _trialists = null;
                NavigationService.Navigate<DataImportViewModel>(this);
            }
        }

        public override async void Prepare(object payload)
        {
            _trialists = new List<Trialist>();
            await foreach (Trialist element in (IAsyncEnumerable<Trialist>)payload)
                _trialists.Add(element);
            await CreateDraw().ConfigureAwait(false);
        }

        protected override async void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(RunsPerDay))
                await CreateDraw().ConfigureAwait(false);
        }
    }
}
