using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using UIConcepts.Core.Model.Context;
using UIConcepts.Core.Model.ContextModel;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("View/Edit Data")]
    public class DataDisplayViewModel : ViewModelBase
    {
        #region Fields

        private readonly ManagerContext _managerContext;

        #endregion

        #region Commands

        public IMvxCommand ImportDataCommand => new MvxCommand(OnImportData);

        #endregion

        #region Properties

        #endregion

        public DataDisplayViewModel(IMvxNavigationService navigationService, IManagerContext managerContext)
            : base(navigationService)
        {
            _managerContext = (ManagerContext)managerContext;
        }

        private async void OnImportData()
        {
            Trialist trialist = new Trialist
            {
                FirstName = DateTime.Now.ToLongTimeString(),
                Surname = "Stephens"
            };
            _managerContext.Trialists.Add(trialist);
            await _managerContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
