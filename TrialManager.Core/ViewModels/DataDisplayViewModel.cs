using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Realms;
using System.IO;
using System.Linq;
using TrialManager.Core.Model;
using TrialManager.Core.Model.Messages;
using TrialManager.Core.Model.TrialistDb;
using TrialManager.Core.Services;
using TrialManager.Core.ViewModels.Base;

namespace TrialManager.Core.ViewModels
{
    [DisplayNavigation("View/Edit Data")]
    public class DataDisplayViewModel : ViewModelBase
    {
        #region Fields

        private readonly Realm _realm;
        private readonly IIntraMessenger _messagingService;
        private readonly IDataImportService _importService;

        private IQueryable<Trialist> _trialists;
        private Trialist _selectedTrialist;
        private Transaction _updateTransaction;
        private bool _isEditDialogOpen;
        private bool _canUseEditControls = true;
        private double _listOpacity = 1;

        #endregion

        #region Commands

        public IMvxCommand ImportDataCommand => new MvxCommand(OnImportDataRequested);

        /// <summary>
        /// Adds a trialist to the data source and saves the DB
        /// </summary>
        public IMvxCommand AddTrialistCommand => new MvxCommand(() => _realm.Write(() => _realm.Add(Trialist.Default)));

        /// <summary>
        /// Removes a trialist from the data source and saves the DB
        /// </summary>
        public IMvxCommand DeleteTrialistCommand => new MvxCommand(async () =>
        {
            if (CanDeleteDataEntries)
            {
                Trialist temp = Trialists.First();
                await AsyncDispatcher.ExecuteOnMainThreadAsync(() => Trialists = null);
                _realm.Write(() => _realm.Remove(temp));
                Trialists = _realm.All<Trialist>();
            }
            //Trialists = _realm.All<Trialist>();
            //RaisePropertyChanged(nameof(Trialists));
            //if (CanDeleteDataEntries)
            //    _realm.Write(() => _realm.Remove(Trialists.First()));
        });

        /// <summary>
        /// Invokes the data edit dialog and starts tracking the edited item
        /// </summary>
        public IMvxCommand EditDataEntryCommand => new MvxCommand(() =>
        {
            _updateTransaction = _realm.BeginWrite();
            IsEditDialogOpen = true;
        });

        /// <summary>
        /// Closes the data edit dialog and saves the DB
        /// </summary>
        public IMvxCommand CloseEditDialogCommand => new MvxCommand(() =>
        {
            _updateTransaction.Commit();
            _updateTransaction.Dispose();
            IsEditDialogOpen = false;
        });

        /// <summary>
        /// Deletes a dog from the currently edited trialist
        /// </summary>
        public IMvxCommand DeleteDogCommand => new MvxCommand<Dog>((d) => SelectedTrialist.SafeRemoveDog(d));

        /// <summary>
        /// Adds a dog to the currently edited trialist
        /// </summary>
        public IMvxCommand AddDogCommand => new MvxCommand(() => SelectedTrialist.Dogs.Add(Dog.Default));

        #endregion

        #region Properties

        /// <summary>
        /// Gets the local view of trialists
        /// </summary>
        public IQueryable<Trialist> Trialists
        {
            get => _trialists;
            set
            {
                SetProperty(ref _trialists, value);
                RaisePropertyChanged(nameof(CanEditDataEntry));
                RaisePropertyChanged(nameof(CanDeleteDataEntries));
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a data entry selection can be edited
        /// </summary>
        public bool CanEditDataEntry => _selectedTrialist != null;

        /// <summary>
        /// Gets a value indicating whether or not data entries can be deleted
        /// </summary>
        public bool CanDeleteDataEntries => _selectedTrialist != null;

        public Trialist SelectedTrialist
        {
            get => _selectedTrialist;
            set
            {
                SetProperty(ref _selectedTrialist, value);
                RaisePropertyChanged(nameof(CanEditDataEntry));
                RaisePropertyChanged(nameof(CanDeleteDataEntries));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the data editing controls. Also controls the visibility of the import progress bar
        /// </summary>
        public bool CanUseEditControls
        {
            get => _canUseEditControls;
            set => SetProperty(ref _canUseEditControls, value);
        }

        /// <summary>
        /// Gets or sets the opacity of the data list
        /// </summary>
        public double ListOpacity
        {
            get => _listOpacity;
            set => SetProperty(ref _listOpacity, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the editing dialog is open
        /// </summary>
        public bool IsEditDialogOpen
        {
            get => _isEditDialogOpen;
            set => SetProperty(ref _isEditDialogOpen, value);
        }

        #endregion

        public DataDisplayViewModel(IMvxNavigationService navigationService,
            IIntraMessenger messagingService,
            IDataImportService importService)
            : base(navigationService)
        {
            _realm = RealmHelpers.GetRealmInstance();
            Trialists = _realm.All<Trialist>();
            _messagingService = messagingService;
            _importService = importService;
        }

        /// <summary>
        /// If required, ask the user if they want to merge data before importing
        /// </summary>
        private void OnImportDataRequested()
        {
            if (Trialists.Any())
            {
                void ResultCallback(DialogMessage.DialogButton d)
                {
                    if ((d & DialogMessage.DialogButton.Yes) != 0)
                        ImportData(true);
                    else
                        ImportData(false);
                }

                DialogMessage dialogRequest = new DialogMessage
                {
                    Buttons = DialogMessage.DialogButton.Yes | DialogMessage.DialogButton.No,
                    Title = "Warning",
                    Content = "Do you wish to merge the import with the current data?",
                    Callback = ResultCallback
                };
                _messagingService.Send(dialogRequest);
            }
            else
            {
                ImportData(false);
            }
        }

        private void ImportData(bool merge)
        {
            async void callback(FileDialogMessage.DialogResult result, string path)
            {
                if (result == FileDialogMessage.DialogResult.Failed)
                    return;

                // Tell the user if they've selected a file that does not exist
                if (!File.Exists(path))
                {
                    _messagingService.Send(new DialogMessage
                    {
                        Title = "Error",
                        Content = "Could not locate that file. Please try again",
                        Buttons = DialogMessage.DialogButton.Ok
                    });
                    return;
                }

                // Prevent the user from editing/adding data while an import is in progress, and dim list
                CanUseEditControls = false;
                ListOpacity = 0.5;

                if (!await _importService.ImportFromCsv(path, merge).ConfigureAwait(false))
                {
                    _messagingService.Send(new DialogMessage
                    {
                        Title = "Error",
                        Content = "There was a problem opening the file. Please make sure that no other programs are using the file, then try again",
                        Buttons = DialogMessage.DialogButton.Ok
                    });
                }

                CanUseEditControls = true;
                ListOpacity = 1;

                //await NavigationService.Navigate<DataImportViewModel, Tuple<string, bool>>(new Tuple<string, bool>(path, merge)).ConfigureAwait(false);
            }

            _messagingService.Send(new FileDialogMessage
            {
                Title = "Import data file",
                Callback = callback
            });
        }
    }
}
