using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("View/Edit Data")]
    public class DataDisplayViewModel : ViewModelBase
    {
        #region Fields

        #endregion

        #region Commands

        public IMvxCommand ImportDataCommand => new MvxCommand(OnImportData);

        #endregion

        #region Properties

        #endregion

        public DataDisplayViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }

        private void OnImportData()
        {
            throw new NotImplementedException();
        }
    }
}
