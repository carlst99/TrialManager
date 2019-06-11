using MvvmCross.Navigation;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("View/Edit Data")]
    public class DataDisplayViewModel : Base.ViewModelBase
    {
        public DataDisplayViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }

        #region Fields

        #endregion

        #region Properties

        #endregion
    }
}
