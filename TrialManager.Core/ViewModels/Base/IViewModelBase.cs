using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TrialManager.Core.ViewModels.Base
{
    public interface IViewModelBase
    {
        string this[string index] { get; }
        IMvxNavigationService NavigationService { get; }
    }
}
