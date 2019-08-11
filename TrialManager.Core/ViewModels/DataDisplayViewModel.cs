using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        private readonly TrialistContext _trialistContext;
        private readonly IIntraMessenger _messagingService;
        private readonly IDataImportService _importService;

        private IList<Trialist> _selectedTrialists;
        private Trialist _trialistToEdit;
        private bool _isEditDialogOpen;

        #endregion

        #region Commands

        public IMvxCommand ImportDataCommand => new MvxCommand(OnImportDataRequested);

        /// <summary>
        /// Updates the list of selected trialists
        /// </summary>
        public IMvxCommand TrialistSelectionChangedCommand => new MvxCommand<IList>((s) =>
        {
            _selectedTrialists = ConvertToList<Trialist>(s);
            RaisePropertyChanged(nameof(CanEditDataEntry));
            RaisePropertyChanged(nameof(CanDeleteDataEntries));
        });

        /// <summary>
        /// Adds a trialist to the data source and saves the DB
        /// </summary>
        public IMvxCommand AddTrialistCommand => new MvxCommand(async () =>
        {
            Trialist trialist = Trialist.Default;
            _trialistContext.Trialists.Add(trialist);
            await _trialistContext.SaveChangesAsync().ConfigureAwait(false);
        });

        /// <summary>
        /// Removes a trialist from the data source and saves the DB
        /// </summary>
        public IMvxCommand DeleteTrialistCommand => new MvxCommand(async () =>
        {
            _trialistContext.Trialists.RemoveRange(_selectedTrialists);
            await _trialistContext.SaveChangesAsync().ConfigureAwait(false);
        });

        /// <summary>
        /// Invokes the data edit dialog and starts tracking the edited item
        /// </summary>
        public IMvxCommand EditDataEntryCommand => new MvxCommand<Trialist>((t) =>
        {
            TrialistToEdit = t;
            _trialistContext.Update(t);
            IsEditDialogOpen = true;
        });

        /// <summary>
        /// Closes the data edit dialog and saves the DB
        /// </summary>
        public IMvxCommand CloseEditDialogCommand => new MvxCommand(async () =>
        {
            IsEditDialogOpen = false;
            await _trialistContext.SaveChangesAsync().ConfigureAwait(false);
        });

        /// <summary>
        /// Deletes a dog from the currently edited trialist
        /// </summary>
        public IMvxCommand DeleteDogCommand => new MvxCommand<Dog>((d) => _trialistToEdit.SafeRemoveDog(d));

        /// <summary>
        /// Adds a dog to the currently edited trialist
        /// </summary>
        public IMvxCommand AddDogCommand => new MvxCommand(() => _trialistToEdit.Dogs.Add(Dog.Default));

        #endregion

        #region Properties

        /// <summary>
        /// Gets the local view of trialists
        /// </summary>
        public ObservableCollection<Trialist> Trialists => _trialistContext.Trialists.Local.ToObservableCollection();

        /// <summary>
        /// Gets a value indicating whether or not a data entry selection can be edited
        /// </summary>
        public bool CanEditDataEntry => _selectedTrialists?.Count == 1;

        /// <summary>
        /// Gets a value indicating whether or not data entries can be deleted
        /// </summary>
        public bool CanDeleteDataEntries => _selectedTrialists?.Count > 0;

        /// <summary>
        /// Gets or sets the <see cref="Trialist"/> object that should be loaded by the editor
        /// </summary>
        public Trialist TrialistToEdit
        {
            get => _trialistToEdit;
            set => SetProperty(ref _trialistToEdit, value);
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
            ITrialistContext TrialistContext,
            IIntraMessenger messagingService,
            IDataImportService importService)
            : base(navigationService)
        {
            _trialistContext = (TrialistContext)TrialistContext;
            _messagingService = messagingService;
            _importService = importService;
        }

        /// <summary>
        /// If required, ask the user if they want to merge data before importing
        /// </summary>
        private void OnImportDataRequested()
        {
            if (Trialists?.Count != 0)
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

                try
                {
                    await _importService.ImportFromCsv(path, merge).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    App.LogError("Could not parse CSV file", ex);
                    _messagingService.Send(new DialogMessage
                    {
                        Title = "Error",
                        Content = "There was a problem opening the file. Please make sure that no other programs are using the file, then try again",
                        Buttons = DialogMessage.DialogButton.Ok
                    });
                }
            }

            _messagingService.Send(new FileDialogMessage
            {
                Title = "Import data file",
                Callback = callback
            });
        }

        private IList<T> ConvertToList<T>(IList list)
        {
            List<T> converted = new List<T>();

            for (int i = 0; i < list.Count; i++)
            {
                converted.Add((T)list[i]);
            }

            return converted;
        }
    }
}
