using Serilog;
using Stylet;
using System;
using TrialManager.Resources;
using TrialManager.Services;

namespace TrialManager.ViewModels.Base
{
    public abstract class ViewModelBase : Screen, IViewModelBase
    {
        public string this[string index] => AppStrings.ResourceManager.GetString(index);
        public IEventAggregator EventAggregator { get; }
        public NavigationService NavigationService { get; }

        protected ViewModelBase(IEventAggregator eventAggregator, NavigationService navigationService)
        {
            EventAggregator = eventAggregator;
            NavigationService = navigationService;

            Log.Verbose("Navigated to " + GetType().Name);
        }

        public virtual void Prepare(object payload)
        {
        }
    }

    public abstract class ViewModelConductorBase : Conductor<Screen>, IViewModelBase
    {
        public string this[string index] => AppStrings.ResourceManager.GetString(index);
        public IEventAggregator EventAggregator { get; }
        public NavigationService NavigationService { get; }

        protected ViewModelConductorBase(IEventAggregator eventAggregator, NavigationService navigationService)
        {
            EventAggregator = eventAggregator;
            NavigationService = navigationService;

            Log.Verbose("Navigated to " + GetType().Name);
            NavigationService.NavigationRequested += OnNavigationRequested;
        }

        public virtual void Prepare(object payload)
        {
        }

        /// <summary>
        /// Default handler for <see cref="NavigationService.NavigationRequested"/>
        /// </summary>
        /// <param name="sender">The request sender</param>
        /// <param name="e">The type to navigate to</param>
        /// <param name="p">The payload, if any</param>
        protected virtual void OnNavigationRequested(object sender, Type e, object p)
        {
        }
    }
}
