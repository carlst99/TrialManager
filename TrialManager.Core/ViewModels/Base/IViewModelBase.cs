using MvvmCross.Navigation;

namespace TrialManager.Core.ViewModels.Base
{
    public interface IViewModelBase
    {
        string this[string index] { get; }
        IMvxNavigationService NavigationService { get; }
    }
}
