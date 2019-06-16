using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIConcepts.Core.Model.Context;
using UIConcepts.Core.Model.ContextModel;
using UIConcepts.Core.Services.CoreMessaging;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("View/Edit Data")]
    public class DataDisplayViewModel : ViewModelBase
    {
        #region Fields

        private readonly ManagerContext _managerContext;
        private List<Trialist> _trialists;
        private ICoreMessagingService _messagingService;

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
                                    ICoreMessagingService messagingService)
            : base(navigationService)
        {
            _managerContext = (ManagerContext)managerContext;
            _messagingService = messagingService;
            _trialists = _managerContext.Trialists.ToList();
        }

        private async void OnImportData()
        {
            //Trialist trialist = new Trialist
            //{
            //    FirstName = DateTime.Now.ToLongTimeString(),
            //    Surname = "Stephens"
            //};
            //_managerContext.Trialists.Add(trialist);
            if (_trialists?.Count != 0)
                _trialists = _managerContext.Trialists.ToList();

            await _managerContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
