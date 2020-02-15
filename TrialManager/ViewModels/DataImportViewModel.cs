using CsvHelper;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Serilog;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model;
using TrialManager.Model.Csv;
using TrialManager.Model.TrialistDb;
using TrialManager.Services;
using TrialManager.ViewModels.Base;
using TrialManager.Views;

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
        private const string DEFAULT_PROPERTY_INDICATOR = "Select Value";

        #region Fields

        private readonly ISnackbarMessageQueue _messageQueue;

        private string _filePath;
        private ReadOnlyCollection<string> _csvHeaders;
        private BindableCollection<PropertyHeaderPair> _mappedProperties;

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

        public BindableCollection<PropertyHeaderPair> MappedProperties
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

        public async Task OpenFileSelectionDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Google Forms Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Open data file"
            };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    if (await DataImportService.IsValidCsvFile(ofd.FileName).ConfigureAwait(false))
                    {
                        FilePath = ofd.FileName;
                        IsImportFileSectionValid = true;
                        PrepareSetupMappingsSection();
                    }
                    else
                    {
                        IsImportFileSectionValid = false;
                        _messageQueue.Enqueue("Please select a valid CSV file");
                    }
                }
                catch (IOException ioex)
                {
                    Log.Error(ioex, "Could not open CSV file");
                    _messageQueue.Enqueue("Could not open the selected file! Please check that no other programs are using it");
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

            MappedProperties = new BindableCollection<PropertyHeaderPair>();
            MappedProperty[] mappedPropertyEnumValues = (MappedProperty[])Enum.GetValues(typeof(MappedProperty));
#if DEBUG
            // Automatically map for debug mode
            for (int i = 0; i < mappedPropertyEnumValues.Length; i++)
            {
                MappedProperty property = mappedPropertyEnumValues[i];
                string header = CsvHeaders[i];
                MappedProperties.Add(new PropertyHeaderPair(property, header));
            }
#else
            foreach (MappedProperty value in Enum.GetValues(typeof(MappedProperty)))
                MappedProperties.Add(new PropertyHeaderPair(value, DEFAULT_PROPERTY_INDICATOR));
#endif
        }

        /// <summary>
        /// Validates each section and moves to the next consecutive section
        /// </summary>
        /// <param name="section">The current section</param>
        /// <returns></returns>
        public async Task ValidateAndContinue(ImportSection section)
        {
            switch (section)
            {
                case ImportSection.ImportFile:
                    if (!await DataImportService.IsValidCsvFile(FilePath).ConfigureAwait(false))
                    {
                        _messageQueue.Enqueue("Please select a valid CSV file!");
                        return;
                    }
                    IsImportFileSectionExpanded = false;
                    IsSetupMappingsSectionExpanded = true;
                    break;
                case ImportSection.SetupMappings:
                    if (!await ValidateSetupMappingsSection().ConfigureAwait(false))
                        return;
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

        /// <summary>
        /// When invoked this method will reverse the user to the last setup section
        /// </summary>
        /// <param name="section">The current section</param>
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

        /// <summary>
        /// Validates the user input of the Setup Mappings section and ensures that the CSV file can be successfully parsed
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ValidateSetupMappingsSection()
        {
            // Check for default/no mappings
            foreach (PropertyHeaderPair mapping in MappedProperties)
            {
                if (mapping.DataFileProperty == DEFAULT_PROPERTY_INDICATOR)
                {
                    _messageQueue.Enqueue("Please map every TrialManager property to a property from your CSV file");
                    return false;
                }
            }
            // Check for duplicate/reused mappings
            foreach (PropertyHeaderPair mapping in MappedProperties)
            {
                foreach (PropertyHeaderPair mapping2 in MappedProperties)
                {
                    if (mapping.DataFileProperty == mapping2.DataFileProperty && mapping.MappedProperty != mapping2.MappedProperty)
                    {
                        _messageQueue.Enqueue("You have duplicate mappings!");
                        return false;
                    }
                }
            }
            try
            {
                CsvReader csvReader = null;
                try
                {
                    csvReader = DataImportService.GetCsvReader(FilePath, CreateClassMap());
                }
                catch (IOException ioex)
                {
                    Log.Error(ioex, "Could not open CSV file");
                    MessageDialog messageDialog = new MessageDialog()
                    {
                        Title = "File Error",
                        Message = "TrialManager could not open the CSV file that you have selected. Please ensure that no other programs are using the file and try again",
                        HelpUrl = null
                    };
                    await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
                    return false;
                }
                csvReader.Read();
                MappedTrialist record = csvReader.GetRecord<MappedTrialist>();

                // Check that the enum value has been parsed
                if (record.Status == EntityStatus.None)
                {
                    // Build a string list of all the possible EntityStatus values, to inform the user
                    string statusCombination = string.Empty;
                    string[] statusEnumNames = Enum.GetNames(typeof(EntityStatus));
                    for (int i = 0; i < statusEnumNames.Length; i++)
                    {
                        if (statusEnumNames[i] != nameof(EntityStatus.None))
                        {
                            statusCombination += statusEnumNames[i];
                            if (i + 1 != statusEnumNames.Length)
                                statusCombination += ", ";
                        }
                    }
                    MessageDialog messageDialog = new MessageDialog()
                    {
                        Title = "Status Parse Error",
                        Message = "TrialManager could not parse the status columns in your CSV file. Please make sure that each status entry matches one of the following values: " + statusCombination
                    };
                    await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not read CSV file");
                MessageDialog messageDialog = new MessageDialog()
                {
                    Message = "TrialManager could not import data from this CSV file. Please check that you have:\n  • Selected the correct CSV file\n  • Correctly mapped the CSV file properties to TrialManager properties"
                };
                await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
                return false;
            }
        }

        /// <summary>
        /// Creates a class map based on user mappings input
        /// </summary>
        /// <returns></returns>
        private TrialistCsvClassMap CreateClassMap()
        {
            TrialistCsvClassMap classMap = new TrialistCsvClassMap();
            System.Reflection.PropertyInfo[] properties = classMap.GetType().GetProperties();
            foreach (PropertyHeaderPair pair in MappedProperties)
            {
                System.Reflection.PropertyInfo property = properties.First(p => p.Name == pair.MappedProperty.ToString());
                property.SetValue(classMap, pair.DataFileProperty);
            }
            classMap.SetupMappings();
            return classMap;
        }
    }
}
