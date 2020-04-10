using Stylet;
using System;

namespace TrialManager.Services
{
    public delegate void NavigationEventHandler<TViewModel, TPayload>(object sender, TViewModel e, TPayload p);

    public class NavigationService
    {
        /// <summary>
        /// Invoked when a navigation is requested
        /// </summary>
        public event NavigationEventHandler<Type, object> NavigationRequested;

        /// <summary>
        /// Navigates to a viewmodel
        /// </summary>
        /// <typeparam name="TViewModel">The type of viewmodel to navigate to</typeparam>
        public void Navigate<TViewModel>(object sender) where TViewModel : Screen
        {
            NavigationRequested?.Invoke(sender, typeof(TViewModel), null);
        }

        /// <summary>
        /// Navigates to a viewmodel and passes the selected payload
        /// </summary>
        /// <typeparam name="TViewModel">The type of viewmodel to navigate to</typeparam>
        /// <typeparam name="TPayload">The type of payload to pass</typeparam>
        /// <param name="payload">The payload to pass to the viewmodel</param>
        public void Navigate<TViewModel, TPayload>(object sender, TPayload payload) where TViewModel : Screen
        {
            NavigationRequested?.Invoke(sender, typeof(TViewModel), payload);
        }
    }
}
