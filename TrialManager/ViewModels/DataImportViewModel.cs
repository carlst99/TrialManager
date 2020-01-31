using Microsoft.WindowsAPICodePack.Dialogs;
using Stylet;
using System;
using System.IO;
using TrialManager.Services;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public class DataImportViewModel : ViewModelBase
    {
        private string _filePath;

        #region Properties

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
