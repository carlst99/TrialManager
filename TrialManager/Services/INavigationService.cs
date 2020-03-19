using Stylet;
using System;

namespace TrialManager.Services
{
    public delegate void NavigationEventHandler<TViewModel, TPayload>(object sender, TViewModel e, TPayload p);

    public interface INavigationService
    {
        event NavigationEventHandler<Type, object> NavigationRequested;
        void Navigate<TViewModel>(object sender) where TViewModel : Screen;
        void Navigate<TViewModel, TPayload>(object sender, TPayload payload) where TViewModel : Screen;
    }
}
