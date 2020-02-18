using Stylet;
using TrialManager.Services;

namespace TrialManager.ViewModels.Base
{
    public interface IViewModelBase : IScreen
    {
        string this[string index] { get; }
        IEventAggregator EventAggregator { get; }
        INavigationService NavigationService { get; }
        void Prepare(object payload);
    }
}
