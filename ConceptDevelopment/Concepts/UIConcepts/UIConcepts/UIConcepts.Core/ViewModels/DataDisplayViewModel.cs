using MvvmCross.Navigation;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("View/Edit Data")]
    public class DataDisplayViewModel : ViewModelBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        public DataDisplayViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
