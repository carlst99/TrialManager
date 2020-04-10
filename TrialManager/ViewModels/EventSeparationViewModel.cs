using MaterialDesignThemes.Wpf;
using Serilog;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TrialManager.Resources;
using TrialManager.Services;
using TrialManager.Views;

namespace TrialManager.ViewModels
{
    public class EventSeparationViewModel : Base.ViewModelBase
    {
        private readonly CsvImportService _importService;
        private readonly EventSeparatorService _separator;

        private ReadOnlyCollection<string> _csvHeaders;
        private string _filePath;
        private string _eventsHeader;

        /// <summary>
        /// Gets or sets the headers contained in the CSV file
        /// </summary>
        public ReadOnlyCollection<string> CsvHeaders
        {
            get => _csvHeaders;
            set => SetAndNotify(ref _csvHeaders, value);
        }

        /// <summary>
        /// Gets or sets the header that targets the events column
        /// </summary>
        public string EventsHeader
        {
            get => _eventsHeader;
            set => SetAndNotify(ref _eventsHeader, value);
        }

        public EventSeparationViewModel(
            CsvImportService importService,
            EventAggregator eventAggregator,
            EventSeparatorService separator,
            INavigationService navigationService)
            : base(eventAggregator, navigationService)
        {
            _importService = importService;
            _separator = separator;
        }

        public async Task Separate()
        {
            try
            {
                _separator.Separate(_filePath, EventsHeader);
            }
            catch (IOException ioex)
            {
                Log.Error(ioex, "Could not read/write CSV file");
                await DisplayIOExceptionDialog().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not separate events");
                await DisplayUnexpectedExceptionDialog().ConfigureAwait(false);
            }
        }

        public static void NavigateToDocumentation()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = HelpUrls.EventSeparation,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// Displays a <see cref="MessageDialog"/> which alerts the user to an issue with reading the CSV file
        /// </summary>
        private async Task DisplayIOExceptionDialog()
        {
            MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
            {
                Title = "File Error",
                Message = "TrialManager could not read the CSV file that you have selected. Please ensure that no other programs are using the file and try again",
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
                Message = "TrialManager encountered an unexpected exception when separating the events. Please try again",
                HelpUrl = HelpUrls.EventSeparation
            });
            await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
        }

        public override void Prepare(object payload)
        {
            _filePath = (string)payload;
        }

        protected override void OnActivate()
        {
            List<string> headers = new List<string>(_importService.GetCsvHeaders(_filePath));
            CsvHeaders = new ReadOnlyCollection<string>(headers);
            base.OnActivate();
        }
    }
}
