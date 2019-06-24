using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private Guid _lastMessageId;

        #endregion

        #region Commands

        public IMvxCommand ImportDataCommand => new MvxCommand(OnImportData);
        public IMvxCommand EditDataEntryCommand => new MvxCommand(OnEditDataEntry);
        public IMvxCommand TrialistSelectionChangedCommand => new MvxCommand<IList>(OnTrialistSelectionChanged);

        #endregion

        #region Properties

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

        #endregion

        public DataDisplayViewModel(IMvxNavigationService navigationService,
                                    IManagerContext managerContext,
                                    IIntraMessenger messagingService)
            : base(navigationService)
        {
            _managerContext = (ManagerContext)managerContext;
            _messagingService = messagingService;
            _messagingService.Subscribe(OnMessageReceived, new Type[] { typeof(DialogResultMessage) });

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
            //    MessageDialogMessage dialogRequest = new MessageDialogMessage
            //    {
            //        Action = DialogAction.Yes | DialogAction.No,
            //        Id = GetNewMessageId(),
            //        Title = "Warning",
            //        Content = "Do you wish to merge the import with the current data?"
            //    };
            //    _messagingService.Enqueue(dialogRequest);
            //} else
            //{
            //    ImportData(false);
            //}
        }

        private void OnEditDataEntry()
        {

        }

        private void OnTrialistSelectionChanged(IList selection)
        {
            _selectedTrialists = ConvertToList<Trialist>(selection);
            RaisePropertyChanged(nameof(CanEditDataEntry));
        }

        private async void ImportData(bool merge)
        {

        }

        private void OnMessageReceived(IMessage message)
        {
            if (message is DialogResultMessage dMessage && dMessage.Id == _lastMessageId)
            {
                if (dMessage.Result.HasFlag(DialogAction.Yes))
                    ImportData(true);
                else if (dMessage.Result.HasFlag(DialogAction.No))
                    ImportData(false);
            }
        }

        private Guid GetNewMessageId()
        {
            _lastMessageId = Guid.NewGuid();
            return _lastMessageId;
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
