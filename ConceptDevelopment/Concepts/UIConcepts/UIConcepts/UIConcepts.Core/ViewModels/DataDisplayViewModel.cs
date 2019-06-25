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
        public IMvxCommand EditDataEntryCommand => new MvxCommand<Trialist>(OnEditDataEntry);
        public IMvxCommand CloseEditDialogCommand => new MvxCommand(OnEditDialogClose);
        public IMvxCommand TrialistSelectionChangedCommand => new MvxCommand<IList>(OnTrialistSelectionChanged);

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
        public bool CanEditDataEntry
        {
            get
            {
                if (_selectedTrialists?.Count == 1)
                    return true;
                else
                    return false;
            }
        }

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
                _trialists = new ObservableCollection<Trialist>(_managerContext.Trialists.ToList());
            else
                _trialists = new ObservableCollection<Trialist>();
        }

        private async void OnImportData()
        {
            Dog dog = new Dog
            {
                Name = "Flynn",
                Status = EntityStatus.OpenBlack
            };
            Dog[] dogs = new Dog[] { dog, dog, dog };
            Trialist trialist = new Trialist
            {
                FirstName = DateTime.Now.ToLongTimeString(),
                Surname = "Name, sir",
                Email = "gg123@gg123.com",
                PhoneNumber = "0123456789",
                Status = EntityStatus.Maiden,
                Dogs = new ObservableCollection<Dog>(dogs)
            };
            Trialists.Add(trialist);
            _managerContext.Add(trialist);
            await _managerContext.SaveChangesAsync().ConfigureAwait(false);

            //if (_trialists?.Count != 0)
            //{
            //    async void ResultCallback(DialogAction d)
            //    {
            //        if (d.HasFlag(DialogAction.Yes))
            //            await ImportData(true).ConfigureAwait(false);
            //        else
            //            await ImportData(false).ConfigureAwait(false);
            //    }

            //    MessageDialogMessage dialogRequest = new MessageDialogMessage
            //    {
            //        Actions = DialogAction.Yes | DialogAction.No,
            //        Title = "Warning",
            //        Content = "Do you wish to merge the import with the current data?",
            //        Callback = ResultCallback
            //    };
            //    _messagingService.Send(dialogRequest);
            //}
            //else
            //{
            //    await ImportData(false).ConfigureAwait(false);
            //}
        }

        private void OnEditDataEntry(Trialist trialist)
        {
            TrialistToEdit = trialist;
            _managerContext.Update(trialist);
            IsEditDialogOpen = true;
        }

        private async void OnEditDialogClose()
        {
            IsEditDialogOpen = false;
            await _managerContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private void OnTrialistSelectionChanged(IList selection)
        {
            _selectedTrialists = ConvertToList<Trialist>(selection);
            RaisePropertyChanged(nameof(CanEditDataEntry));
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
