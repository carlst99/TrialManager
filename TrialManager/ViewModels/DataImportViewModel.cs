using CsvHelper;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using TrialManager.Model;
using TrialManager.Services;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public enum ImportSection
    {
        ImportFile,
        SetupMappings,
        PreferredDay,
        Duplicates
    }

    public class DataImportViewModel : ViewModelBase
    {
        #region Fields

        private readonly ISnackbarMessageQueue _messageQueue;

        private string _filePath;
        private ReadOnlyCollection<string> _csvHeaders;
        private List<PropertyHeaderPair> _mappedProperties;

        #endregion

        #region Step Validation Fields

        private bool _isImportFileSectionValid;
        private bool _isImportFileSectionExpanded;
        private bool _isSetupMappingsSectionExpanded;
        private bool _isPreferredDaySectionExpanded;
        private bool _isDuplicatesSectionExpanded;

        #endregion

        #region Import File Properties

        /// <summary>
        /// Gets or sets the path to the imported file
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set
            {
                SetAndNotify(ref _filePath, value);
                NotifyOfPropertyChange(nameof(FileName));
            }
        }

        /// <summary>
        /// Gets the name of the imported file
        /// </summary>
        public string FileName => Path.GetFileName(_filePath);

        #endregion

        #region Setup Mappings Properties

        /// <summary>
        /// Gets or sets the headers contained in the CSV file
        /// </summary>
        public ReadOnlyCollection<string> CsvHeaders
        {
            get => _csvHeaders;
            set => SetAndNotify(ref _csvHeaders, value);
        }

        public List<PropertyHeaderPair> MappedProperties
        {
            get => _mappedProperties;
            set => SetAndNotify(ref _mappedProperties, value);
        }

        #endregion

        #region Step Validation Properties

        /// <summary>
        /// Gets or sets a value indicating whether the Import File Section is valid
        /// </summary>
        public bool IsImportFileSectionValid
        {
            get => _isImportFileSectionValid;
            set => SetAndNotify(ref _isImportFileSectionValid, value);
        }

        public bool IsImportFileSectionExpanded
        {
            get => _isImportFileSectionExpanded;
            set => SetAndNotify(ref _isImportFileSectionExpanded, value);
        }

        public bool IsSetupMappingsSectionExpanded
        {
            get => _isSetupMappingsSectionExpanded;
            set => SetAndNotify(ref _isSetupMappingsSectionExpanded, value);
        }

        public bool IsPreferredDaySectionExpanded
        {
            get => _isPreferredDaySectionExpanded;
            set => SetAndNotify(ref _isPreferredDaySectionExpanded, value);
        }

        public bool IsDuplicatesSectionExpanded
        {
            get => _isDuplicatesSectionExpanded;
            set => SetAndNotify(ref _isDuplicatesSectionExpanded, value);
        }

        #endregion

        public DataImportViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            ISnackbarMessageQueue messageQueue)
            : base(eventAggregator, navigationService)
        {
            _messageQueue = messageQueue;
            IsImportFileSectionExpanded = true;
        }

        public async void OpenFileSelectionDialog()
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog()
            {
                Title = "Open data file",
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filters = { new CommonFileDialogFilter("Google Forms File", "*.csv") },
            };
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (await DataImportService.IsValidCsvFile(cofd.FileName).ConfigureAwait(false))
                {
                    FilePath = cofd.FileName;
                    IsImportFileSectionValid = true;
                    PrepareSetupMappingsSection();
                }
                else
                {
                    IsImportFileSectionValid = false;
                    _messageQueue.Enqueue("Please select a valid CSV file");
                }
            }
        }

        public void PrepareSetupMappingsSection()
        {
            if (!File.Exists(FilePath))
            {
                _messageQueue.Enqueue("The file you have selected no longer exists. Please select a file again");
                return;
            }

            using StreamReader reader = new StreamReader(FilePath);
            using CsvReader csv = new CsvReader(reader, CultureInfo.CurrentCulture);

            // Get the headers
            csv.Read();
            csv.ReadHeader();
            string[] headerRecord = csv.Context.HeaderRecord;
            if (headerRecord.Length == 0)
                return;
            CsvHeaders = new ReadOnlyCollection<string>(headerRecord);

           List<PropertyHeaderPair> mappings = new List<PropertyHeaderPair>();
            foreach (MappedProperty value in Enum.GetValues(typeof(MappedProperty)))
                mappings.Add(new PropertyHeaderPair(value, CsvHeaders[0]));
            MappedProperties = mappings;
        }

        public async void ValidateAndContinue(ImportSection section)
        {
            switch (section)
            {
                case ImportSection.ImportFile:
                    if (!await DataImportService.IsValidCsvFile(FilePath).ConfigureAwait(false))
                    {
                        _messageQueue.Enqueue("Please select a valid CSV file!");
                        break;
                    }
                    IsImportFileSectionExpanded = false;
                    IsSetupMappingsSectionExpanded = true;
                    break;
                case ImportSection.SetupMappings:
                    _messageQueue.Enqueue("Setup mappings validation Carl!");
                    IsSetupMappingsSectionExpanded = false;
                    IsPreferredDaySectionExpanded = true;
                    break;
                case ImportSection.PreferredDay:
                    _messageQueue.Enqueue("Setup preferred day validation Carl!");
                    IsPreferredDaySectionExpanded = false;
                    IsDuplicatesSectionExpanded = true;
                    break;
            }
        }

        public void StepBack(ImportSection section)
        {
            switch (section)
            {
                case ImportSection.SetupMappings:
                    IsSetupMappingsSectionExpanded = false;
                    IsImportFileSectionExpanded = true;
                    break;
                case ImportSection.PreferredDay:
                    IsPreferredDaySectionExpanded = false;
                    IsSetupMappingsSectionExpanded = true;
                    break;
                case ImportSection.Duplicates:
                    IsDuplicatesSectionExpanded = false;
                    IsPreferredDaySectionExpanded = true;
                    break;
            }
        }
    }
}
