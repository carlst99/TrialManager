using CsvHelper;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TrialManager.Resources;
using TrialManager.Services;

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

        public void Separate()
        {
            using (CsvReader reader = _importService.GetCsvReader(_filePath))
            {
                throw new NotImplementedException();
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
