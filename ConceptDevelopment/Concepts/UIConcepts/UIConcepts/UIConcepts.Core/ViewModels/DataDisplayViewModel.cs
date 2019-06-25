using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UIConcepts.Core.Model.Context;
using UIConcepts.Core.Model.ContextModel;
using UIConcepts.Core.Model.Messages;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("View/Edit Data")]
    public class DataDisplayViewModel : ViewModelBase
    {
        #region Fields

        private readonly ManagerContext _managerContext;
        private readonly IIntraMessenger _messagingService;

        private ObservableCollection<Trialist> _trialists;
        private IList<Trialist> _selectedTrialists;
        private Trialist _trialistToEdit;
        private bool _isEditDialogOpen;

        #endregion

        #region Commands

        public IMvxCommand ImportDataCommand => new MvxCommand(OnImportData);

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
            Trialists.Add(trialist);
            await _managerContext.AddAsync(trialist).ConfigureAwait(false);
            await _managerContext.SaveChangesAsync().ConfigureAwait(false);
        });

        /// <summary>
        /// Removes a trialist from the data source and saves the DB
        /// </summary>
        public IMvxCommand DeleteTrialistCommand => new MvxCommand(async () =>
        {
            _managerContext.RemoveRange(_selectedTrialists);
            foreach (Trialist t in _selectedTrialists)
                Trialists.Remove(t);
            await _managerContext.SaveChangesAsync();
        });

        /// <summary>
        /// Invokes the data edit dialog and starts tracking the edited item
        /// </summary>
        public IMvxCommand EditDataEntryCommand => new MvxCommand<Trialist>((t) =>
        {
            TrialistToEdit = t;
            _managerContext.Update(t);
            IsEditDialogOpen = true;
        });

        /// <summary>
        /// Closes the data edit dialog and saves the DB
        /// </summary>
        public IMvxCommand CloseEditDialogCommand => new MvxCommand(async () =>
        {
            IsEditDialogOpen = false;
            await _managerContext.SaveChangesAsync().ConfigureAwait(false);
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
        /// Gets or sets the list of trialists held in the database
        /// </summary>
        public ObservableCollection<Trialist> Trialists
        {
            get => _trialists;
            set => SetProperty(ref _trialists, value);
        }

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

        public DataDisplayViewModel(IMvxNavigationService navigationService, IManagerContext managerContext, IIntraMessenger messagingService)
            : base(navigationService)
        {
            _managerContext = (ManagerContext)managerContext;
            _messagingService = messagingService;

            if (_managerContext.Trialists.Any())
                Trialists = new ObservableCollection<Trialist>(_managerContext.Trialists.ToList());
            else
                Trialists = new ObservableCollection<Trialist>();
        }

        private async void OnImportData()
        {
            if (_trialists?.Count != 0)
            {
                async void ResultCallback(DialogAction d)
                {
                    if (d.HasFlag(DialogAction.Yes))
                        await ImportData(true).ConfigureAwait(false);
                    else
                        await ImportData(false).ConfigureAwait(false);
                }

                MessageDialogMessage dialogRequest = new MessageDialogMessage
                {
                    Actions = DialogAction.Yes | DialogAction.No,
                    Title = "Warning",
                    Content = "Do you wish to merge the import with the current data?",
                    Callback = ResultCallback
                };
                _messagingService.Send(dialogRequest);
            }
            else
            {
                await ImportData(false).ConfigureAwait(false);
            }
        }

        private async Task ImportData(bool merge)
        {
            throw new NotImplementedException();
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
