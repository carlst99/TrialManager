using MvvmCross.Navigation;

namespace TrialManager.Core.ViewModels
{
    public class HomeViewModel : Base.ViewModelBase
    {
        public HomeViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
