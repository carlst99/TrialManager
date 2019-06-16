using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
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
        private readonly IIntraMessager _messagingService;
        private List<Trialist> _trialists;
        private Guid _lastMessageId;

        #endregion

        #region Commands

        public IMvxCommand ImportDataCommand => new MvxCommand(OnImportData);

        #endregion

        #region Properties

        public List<Trialist> Trialists
        {
            get => _trialists;
            set => SetProperty(ref _trialists, value);
        }

        #endregion

        public DataDisplayViewModel(IMvxNavigationService navigationService,
                                    IManagerContext managerContext,
                                    IIntraMessager messagingService)
            : base(navigationService)
        {
            _managerContext = (ManagerContext)managerContext;
            _messagingService = messagingService;
            _messagingService.Subscribe(OnMessageReceived, Guid.Empty, new Type[] { typeof(DialogResultMessage) });

            //_trialists = _managerContext.Trialists.ToList();
        }

        private void OnImportData()
        {
            if (_trialists?.Count != 0)
            {
                MessageDialogMessage dialogRequest = new MessageDialogMessage
                {
                    Action = DialogAction.Yes | DialogAction.No,
                    Id = GetNewMessageId(),
                    Title = "Warning",
                    Content = "Do you wish to merge the import with the current data?"
                };
                _messagingService.Enqueue(dialogRequest);
            } else
            {
                ImportData(false);
            }
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
    }
}
