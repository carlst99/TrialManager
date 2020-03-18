using CsvHelper;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Serilog;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TrialManager.Model;
using TrialManager.Model.Csv;
using TrialManager.Model.TrialistDb;
using TrialManager.Resources;
using TrialManager.Services;
using TrialManager.Utils;
using TrialManager.ViewModels.Base;
using TrialManager.Views;

namespace TrialManager.ViewModels
{
    public enum ImportSection
    {
        ImportFile,
        SetupMappings,
        SetupOptionalMappings,
        PreferredDay,
        Duplicates
    }

    public class DataImportViewModel : ViewModelBase
    {
        private const string DEFAULT_PROPERTY_INDICATOR = "SELECT VALUE";

        #region Fields

        private readonly ICsvImportService _importService;
        private readonly ISnackbarMessageQueue _messageQueue;

        private Cursor _windowCursor;
        private string _filePath;
        private ReadOnlyCollection<string> _csvHeaders;
        private BindableCollection<PropertyHeaderPair> _mappedProperties;
        private BindableCollection<PropertyHeaderPair> _optionalMappedProperties;
        private BindableCollection<DuplicateTrialistPair> _duplicateTrialistPairs;
        private BindableCollection<PreferredDayDateTimePair> _preferredDayMappings;

        #endregion

        #region Properties

        public Cursor WindowCursor
        {
            get => _windowCursor;
            set => SetAndNotify(ref _windowCursor, value);
        }

        public BindableCollection<DuplicateTrialistPair> DuplicateTrialistPairs
        {
            get => _duplicateTrialistPairs;
            set => SetAndNotify(ref _duplicateTrialistPairs, value);
        }

        public BindableCollection<PreferredDayDateTimePair> PreferredDayMappings
        {
            get => _preferredDayMappings;
            set => SetAndNotify(ref _preferredDayMappings, value);
        }

        #endregion

        #region Step Validation Fields

        private bool _isImportFileSectionValid;
        private bool _isImportFileSectionExpanded;
        private bool _isSetupMappingsSectionExpanded;
        private bool _isSetupOptionalMappingsSectionExpanded;
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

        public BindableCollection<PropertyHeaderPair> OptionalMappedProperties
        {
            get => _optionalMappedProperties;
            set => SetAndNotify(ref _optionalMappedProperties, value);
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

        public bool IsSetupOptionalMappingsSectionExpanded
        {
            get => _isSetupOptionalMappingsSectionExpanded;
            set => SetAndNotify(ref _isSetupOptionalMappingsSectionExpanded, value);
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
            ICsvImportService importService,
            ISnackbarMessageQueue messageQueue)
            : base(eventAggregator, navigationService)
        {
            _importService = importService;
            _messageQueue = messageQueue;
            IsImportFileSectionExpanded = true;
            WindowCursor = Cursors.Arrow;
        }

        #region UI Methods

        public async Task OpenFileSelectionDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Google Forms Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Open data file",
            };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    if (await _importService.IsValidCsvFile(ofd.FileName).ConfigureAwait(false))
                    {
                        FilePath = ofd.FileName;
                        IsImportFileSectionValid = true;
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
                    await DisplayIOExceptionDialog().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Validates each section and moves to the next consecutive section
        /// </summary>
        /// <param name="section">The current section</param>
        /// <returns></returns>
        public async Task ValidateAndContinue(ImportSection section)
        {
            WindowCursor = Cursors.Wait;
            switch (section)
            {
                case ImportSection.ImportFile:
                    IsImportFileSectionExpanded = false;
                    IsSetupMappingsSectionExpanded = true;
                    if (!await PrepareSetupMappingsSection().ConfigureAwait(false))
                        break;
                    break;
                case ImportSection.SetupMappings:
                    if (!await ValidateSetupMappingsSection().ConfigureAwait(false))
                        break;
                    IsSetupMappingsSectionExpanded = false;
                    IsSetupOptionalMappingsSectionExpanded = true;
                    break;
                case ImportSection.SetupOptionalMappings:
                    if (!ValidateSetupOptionalMappingsSection())
                        break;
                    IsSetupOptionalMappingsSectionExpanded = false;
                    // Check if a preferred day mapping has been set
                    if (OptionalMappedProperties.First(p => p.OptionalMappedProperty == OptionalMappedProperty.PreferredDay).DataFileProperty == DEFAULT_PROPERTY_INDICATOR)
                    {
                        IsDuplicatesSectionExpanded = true;
                        await PrepareDuplicatesSection().ConfigureAwait(false);

                        // Move on if there are no duplicates
                        if (DuplicateTrialistPairs.Count == 0)
                            await ValidateAndContinue(ImportSection.Duplicates).ConfigureAwait(false);
                    } else
                    {
                        IsPreferredDaySectionExpanded = true;
                        await PreparePreferredDaySection().ConfigureAwait(false);
                    }
                    break;
                case ImportSection.PreferredDay:
                    if (!await ValidatePreferredDaySection().ConfigureAwait(false))
                        break;
                    IsPreferredDaySectionExpanded = false;
                    IsDuplicatesSectionExpanded = true;
                    await PrepareDuplicatesSection().ConfigureAwait(false);

                    // Move on if there are no duplicates
                    if (DuplicateTrialistPairs.Count == 0)
                        await ValidateAndContinue(ImportSection.Duplicates).ConfigureAwait(false);
                    break;
                case ImportSection.Duplicates:
                    try
                    {
                        IAsyncEnumerable<Trialist> trialists = _importService.BuildTrialistList(DuplicateTrialistPairs, PreferredDayMappings);
                        bool locationEnabled = OptionalMappedProperties.First(p => p.OptionalMappedProperty == OptionalMappedProperty.Address).DataFileProperty != DEFAULT_PROPERTY_INDICATOR;
                        DrawDisplayParams p = new DrawDisplayParams(trialists, locationEnabled);
                        NavigationService.Navigate<DrawDisplayViewModel, DrawDisplayParams>(this, p);
                        IsDuplicatesSectionExpanded = false;
                        IsImportFileSectionExpanded = true;
                        DuplicateTrialistPairs = null;
                        PreferredDayMappings = null;
                        FilePath = string.Empty;
                        MappedProperties = null;
                        CsvHeaders = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Could not finalise trialist list");
                    }
                    break;
            }
            WindowCursor = Cursors.Arrow;
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
                case ImportSection.SetupOptionalMappings:
                    IsSetupOptionalMappingsSectionExpanded = false;
                    IsSetupMappingsSectionExpanded = true;
                    break;
                case ImportSection.PreferredDay:
                    IsPreferredDaySectionExpanded = false;
                    IsSetupOptionalMappingsSectionExpanded = true;
                    break;
                case ImportSection.Duplicates:
                    IsDuplicatesSectionExpanded = false;
                    IsPreferredDaySectionExpanded = true;
                    break;
            }
        }

        public static void ResetDateCommand(PreferredDayDateTimePair pair)
        {
            pair.Day = DateTime.MinValue;
        }

        #endregion

        /// <summary>
        /// Prepares the Setup Mappings section
        /// </summary>
        /// <returns>A value indicating whether preparation was successful</returns>
        private async Task<bool> PrepareSetupMappingsSection()
        {
            try
            {
                List<string> headers = new List<string>(_importService.GetCsvHeaders(FilePath))
                {
                    DEFAULT_PROPERTY_INDICATOR
                };
                CsvHeaders = new ReadOnlyCollection<string>(headers);
            }
            catch (IOException ioex)
            {
                Log.Error(ioex, "Could not read CSV file");
                await DisplayIOExceptionDialog().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not read CSV header record");
                await DisplayUnexpectedExceptionDialog().ConfigureAwait(false);
            }

            MappedProperties = new BindableCollection<PropertyHeaderPair>();
            foreach (MappedProperty element in (MappedProperty[])Enum.GetValues(typeof(MappedProperty)))
            {
                List<double> similarity = CsvHeaders.Select(h => h.CompareStrings(element.ToString())).ToList();
                double bestMatchPercentage = similarity.Max();

                // Only take the best match if it is >= 60% similar
                string bestMatch;
                if (bestMatchPercentage >= 0.6)
                    bestMatch = CsvHeaders[similarity.IndexOf(bestMatchPercentage)];
                else
                    bestMatch = DEFAULT_PROPERTY_INDICATOR;

                PropertyHeaderPair headerPair = new PropertyHeaderPair(element, bestMatch);
                MappedProperties.Add(headerPair);
            }
            OptionalMappedProperties = new BindableCollection<PropertyHeaderPair>();
            foreach (OptionalMappedProperty element in (OptionalMappedProperty[])Enum.GetValues(typeof(OptionalMappedProperty)))
            {
                List<double> similarity = CsvHeaders.Select(h => h.CompareStrings(element.ToString())).ToList();
                double bestMatchPercentage = similarity.Max();

                // Only take the best match if it is >= 60% similar
                string bestMatch;
                if (bestMatchPercentage >= 0.6)
                    bestMatch = CsvHeaders[similarity.IndexOf(bestMatchPercentage)];
                else
                    bestMatch = DEFAULT_PROPERTY_INDICATOR;

                PropertyHeaderPair headerPair = new PropertyHeaderPair(element, bestMatch);
                OptionalMappedProperties.Add(headerPair);
            }
            return true;
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
            CsvReader csvReader = null;
            try
            {
                csvReader = _importService.GetCsvReader(FilePath, _importService.BuildClassMap(MappedProperties, null, DEFAULT_PROPERTY_INDICATOR));
                // Check that we can read records
                csvReader.Read();
                MappedTrialist record = csvReader.GetRecord<MappedTrialist>();
                return true;
            }
            catch (IOException ioex)
            {
                Log.Error(ioex, "Could not read CSV file");
                await DisplayIOExceptionDialog().ConfigureAwait(false);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not read data from CSV file");
                MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
                {
                    Message = "TrialManager could not read data from this CSV file. Please check that you have:\n  • Selected the correct CSV file\n  • Correctly mapped the CSV file properties to TrialManager properties"
                });
                await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
                return false;
            }
            finally
            {
                csvReader.Dispose();
            }
        }

        private bool ValidateSetupOptionalMappingsSection()
        {
            // Check for duplicate/reused mappings
            foreach (PropertyHeaderPair mapping in OptionalMappedProperties)
            {
                foreach (PropertyHeaderPair mapping2 in OptionalMappedProperties)
                {
                    if (mapping.DataFileProperty == mapping2.DataFileProperty && mapping.OptionalMappedProperty != mapping2.OptionalMappedProperty && mapping.DataFileProperty != DEFAULT_PROPERTY_INDICATOR)
                    {
                        _messageQueue.Enqueue("You have duplicate optional mappings!");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Prepares the Preferred Day section
        /// </summary>
        private async Task PreparePreferredDaySection()
        {
            try
            {
                PreferredDayMappings = new BindableCollection<PreferredDayDateTimePair>();
                await foreach (string element in _importService.GetDistinctPreferredDays(FilePath, _importService.BuildClassMap(MappedProperties, OptionalMappedProperties, DEFAULT_PROPERTY_INDICATOR)))
                    PreferredDayMappings.Add(new PreferredDayDateTimePair(element));
            }
            catch (IOException ioex)
            {
                Log.Error(ioex, "Could not read CSV file");
                await DisplayIOExceptionDialog().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not setup preferred day mappings");
                await DisplayUnexpectedExceptionDialog().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Validates the Preferred Day section
        /// </summary>
        private async Task<bool> ValidatePreferredDaySection()
        {
            bool isAtLeastOneSet = false;
            foreach (PreferredDayDateTimePair pair in PreferredDayMappings)
            {
                if (!pair.Day.Equals(DateTime.MinValue))
                    isAtLeastOneSet = true;
            }
            if (!isAtLeastOneSet)
            {
                MessageDialog messageDialog = new MessageDialog(new MessageDialogViewModel
                {
                    Message = "You have left all preferred day mappings on the default value. This means that no the preferred day of each trialist will not be respected when TrialManager puts them in the draw. Are you sure you want to continue?",
                    OkayButtonContent = "Yes",
                    CancelButtonContent = "No",
                    HelpUrl = HelpUrls.PreferredDayMapping
                });
                return (bool)await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
            }
            return true;
        }

        /// <summary>
        /// Prepares the Duplicates section
        /// </summary>
        private async Task PrepareDuplicatesSection()
        {
            try
            {
                DuplicateTrialistPairs = new BindableCollection<DuplicateTrialistPair>();
                await foreach (DuplicateTrialistPair element in _importService.GetMappedDuplicates(FilePath, _importService.BuildClassMap(MappedProperties, OptionalMappedProperties, DEFAULT_PROPERTY_INDICATOR)))
                    DuplicateTrialistPairs.Add(element);
            }
            catch (IOException ioex)
            {
                Log.Error(ioex, "Could not read CSV file");
                await DisplayIOExceptionDialog().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not prepare duplicates section");
                await DisplayUnexpectedExceptionDialog().ConfigureAwait(false);
            }
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
                Message = "TrialManager encountered an unexpected exception when reading the CSV file. Please try again",
                HelpUrl = HelpUrls.Default
            });
            await DialogHost.Show(messageDialog, "MainDialogHost").ConfigureAwait(false);
        }
    }
}
