using CsvHelper;
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
    public class DataImportViewModel : ViewModelBase
    {
        private string _filePath;
        private ReadOnlyCollection<string> _csvHeaders;
        private List<PropertyHeaderPair> _mappedProperties;

        #region Import Data Properties

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
                NotifyOfPropertyChange(nameof(IsImportDataStepValid));
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

        public bool IsImportDataStepValid => CheckValidCsvFile();

        #endregion

        public DataImportViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
            : base(eventAggregator, navigationService)
        {
        }

        public void OpenFileSelectionDialog()
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog()
            {
                Title = "Open data file",
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filters = { new CommonFileDialogFilter("Google Forms File", "*.csv") },
            };
            switch (cofd.ShowDialog())
            {
                case CommonFileDialogResult.Ok:
                    FilePath = cofd.FileName;
                    break;
            }
            PrepareSetupMappings();
        }

        public void PrepareSetupMappings()
        {
            if (!File.Exists(FilePath))
                MessageBox.Show("The file you have selected no longer exists. Please select a file again");

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

        /// <summary>
        /// Performs a basic check for a CSV file by looking for the separator chars (; or ,)
        /// </summary>
        /// <returns></returns>
        private bool CheckValidCsvFile()
        {
            if (string.IsNullOrEmpty(FilePath))
                return false;

            using StreamReader sr = new StreamReader(FilePath);
            string line = sr.ReadLine();
            return line.Contains(';') || line.Contains(',');
        }
    }
}
