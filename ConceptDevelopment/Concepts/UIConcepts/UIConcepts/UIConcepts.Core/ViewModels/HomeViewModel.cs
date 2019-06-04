using MvvmCross.Navigation;

namespace UIConcepts.Core.ViewModels
{
    public class HomeViewModel : Base.ViewModelBase
    {
        public HomeViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
