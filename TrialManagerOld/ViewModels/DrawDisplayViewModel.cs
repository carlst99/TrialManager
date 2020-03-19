using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Realms;
using Serilog;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model;
using TrialManager.Model.Draw;
using TrialManager.Model.TrialistDb;
using TrialManager.Resources;
using TrialManager.Services;
using TrialManager.Utils;
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
        private readonly IPrintService _printService;

        private List<Trialist> _trialists;
        private List<TrialistDrawEntry> _draw;

        private readonly Realm _realmInstance;
        private readonly Preferences _preferences;
        private DrawCreationOptions _drawCreationOptions;

        private bool _showProgress;
        private bool _preparationComplete;
        private bool _isDrawOptionsDialogOpen;
        private bool _showAddressBar;

        #endregion

        #region Properties

        public List<TrialistDrawEntry> Draw
        {
            get => _draw;
            set => SetAndNotify(ref _draw, value);
        }

        public DrawCreationOptions DrawCreationOptions
        {
            get => _drawCreationOptions;
            set => SetAndNotify(ref _drawCreationOptions, value);
        }

        public bool ShowProgress
        {
            get => _showProgress;
            set => SetAndNotify(ref _showProgress, value);
        }

        public bool IsDrawOptionsDialogOpen
        {
            get => _isDrawOptionsDialogOpen;
            set => SetAndNotify(ref _isDrawOptionsDialogOpen, value);
        }

        public bool ShowAddressBar
        {
            get => _showAddressBar;
            set => SetAndNotify(ref _showAddressBar, value);
        }

        #endregion

        public DrawDisplayViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            IDataExportService exportService,
            IDrawCreationService drawService,
            IPrintService printService,
            ISnackbarMessageQueue messageQueue)
            : base(eventAggregator, navigationService)
        {
            _exportService = exportService;
            _drawService = drawService;
            _messageQueue = messageQueue;
            _printService = printService;

            _realmInstance = RealmHelpers.GetRealmInstance();
            _preferences = RealmHelpers.GetUserPreferences(_realmInstance);
            DrawCreationOptions = _preferences.DrawCreationOptions;
            DrawCreationOptions.PropertyChanged += OnDrawCreationOptionsChanged;
        }

        public async Task CreateDraw()
        {
            if (!_preparationComplete)
                return;

            ShowProgress = true;
            try
            {
                Draw = await Task.Run(() => _drawService.CreateDraw(_trialists, DrawCreationOptions).ToList()).ConfigureAwait(true);
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

        public async Task PrintDraw()
        {
            try
            {
                if (_printService.Print(Draw, null))
                    _messageQueue.Enqueue("Draw printed successfully!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not print draw");
                await DisplayUnexpectedExceptionDialog().ConfigureAwait(false);
            }
        }

        public void ToggleDrawOptionsDialog() => IsDrawOptionsDialogOpen = !IsDrawOptionsDialogOpen;

        public override async void Prepare(object payload)
        {
            DrawDisplayParams p = (DrawDisplayParams)payload;
            _trialists = new List<Trialist>();
            await foreach (Trialist element in p.Trialists)
                _trialists.Add(element);
            ShowAddressBar = p.LocationSortingEnabled;
            _preparationComplete = true;
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

        private async void OnDrawCreationOptionsChanged(object sender, EventArgs e)
        {
            _realmInstance.Write(() => _preferences.DrawCreationOptions = DrawCreationOptions);
            await CreateDraw().ConfigureAwait(false);
        }
    }
}
