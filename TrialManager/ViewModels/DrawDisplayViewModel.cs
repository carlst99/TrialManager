using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Serilog;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model;
using TrialManager.Model.TrialistDb;
using TrialManager.Resources;
using TrialManager.Services;
using TrialManager.ViewModels.Base;
using TrialManager.Views;

namespace TrialManager.ViewModels
{
    public class DrawDisplayViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDataExportService _exportService;
        private readonly IDrawCreationService _drawService;
        private readonly ISnackbarMessageQueue _messageQueue;

        private List<Trialist> _trialists;
        private List<TrialistDrawEntry> _draw;
        private string _trialAddress;
        private int _runsPerDay;
        private int _minRunSeparation = 5;
        private int _maxDogsPerDay = 3;
        private bool _showProgress;
        private bool _preparationComplete;

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

        public int MinRunSeparation
        {
            get => _minRunSeparation;
            set => SetAndNotify(ref _minRunSeparation, value);
        }

        public int MaxDogsPerDay
        {
            get => _maxDogsPerDay;
            set => SetAndNotify(ref _maxDogsPerDay, value);
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
            IDataExportService exportService,
            IDrawCreationService drawService,
            ISnackbarMessageQueue messageQueue)
            : base(eventAggregator, navigationService)
        {
            _exportService = exportService;
            _drawService = drawService;
            _messageQueue = messageQueue;
            _runsPerDay = 100;
        }

        public async Task CreateDraw()
        {
            if (!_preparationComplete)
                return;

            ShowProgress = true;
            try
            {
                Draw = await Task.Run(() => _drawService.CreateDraw(_trialists, RunsPerDay, TrialAddress, MinRunSeparation, MaxDogsPerDay).ToList()).ConfigureAwait(true);
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
                _preparationComplete = false;
                NavigationService.Navigate<DataImportViewModel>(this);
            }
        }

        public async Task ExportToCsv()
        {
            SaveFileDialog sfd = GetSaveFileDialog("CSV File (*.csv)|*.csv");
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    await _exportService.ExportAsCsv(Draw, sfd.FileName).ConfigureAwait(false);
                    _messageQueue.Enqueue("Draw exported to CSV successfully!");
                }
                catch (IOException ex)
                {
                    Log.Error("Could not export CSV file", ex);
                    await DisplayIOExceptionDialog().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not export CSV file", ex);
                    await DisplayUnexpectedExceptionDialog().ConfigureAwait(false);
                }
            }
        }

        public async Task ExportToTxt()
        {
            SaveFileDialog sfd = GetSaveFileDialog("Text File (*.txt)|*.txt");
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    await _exportService.ExportAsTxt(Draw, sfd.FileName).ConfigureAwait(false);
                    _messageQueue.Enqueue("Draw exported to text file successfully!");
                }
                catch (IOException ex)
                {
                    Log.Error("Could not export TXT file", ex);
                    await DisplayIOExceptionDialog().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not export TXT file", ex);
                    await DisplayUnexpectedExceptionDialog().ConfigureAwait(false);
                }
            }
        }

        public void PrintDraw()
        {
            _messageQueue.Enqueue("Draw printing is not supported yet!");
        }

        public override async void Prepare(object payload)
        {
            _trialists = new List<Trialist>();
            await foreach (Trialist element in (IAsyncEnumerable<Trialist>)payload)
                _trialists.Add(element);
            _preparationComplete = true;
            await CreateDraw().ConfigureAwait(false);
        }

        protected override async void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(RunsPerDay))
                await CreateDraw().ConfigureAwait(false);
        }

        private SaveFileDialog GetSaveFileDialog(string filter)
        {
            return new SaveFileDialog()
            {
                Filter = filter,
                Title = "Export Draw",
                FileName = "Draw",
                AddExtension = true,
                ValidateNames = true
            };
        }

        /// <summary>
        /// Displays a <see cref="MessageDialog"/> which alerts the user to an issue with reading the CSV file
        /// </summary>
        private async Task DisplayIOExceptionDialog()
        {
            MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
            {
                Title = "File Error",
                Message = "TrialManager could not write the file that you have selected. Please ensure that the file does not already exist, and that you have permission to save to the selected location",
                HelpUrl = HelpUrls.IOException
            });
            await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
        }

        /// <summary>
        /// Displays a <see cref="MessageDialog"/> which alers the user to an unexpected exception
        /// </summary>
        private async Task DisplayUnexpectedExceptionDialog()
        {
            MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
            {
                Title = "Woah!",
                Message = "TrialManager encountered an unexpected exception when exporting the draw. Please try again",
                HelpUrl = HelpUrls.Default
            });
            await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
        }
    }
}
