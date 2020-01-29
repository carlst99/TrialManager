using Realms;
using Serilog;
using Stylet;
using System;
using TrialManager.Model;
using TrialManager.Resources;
using TrialManager.Services;

namespace TrialManager.ViewModels.Base
{
    public abstract class ViewModelBase : Screen, IViewModelBase
    {
        private Realm _realmInstance;

        public string this[string index] => AppStrings.ResourceManager.GetString(index);
        public IEventAggregator EventAggregator { get; }
        public INavigationService NavigationService { get; }

        public Realm RealmInstance
        {
            get => _realmInstance ?? (_realmInstance = RealmHelpers.GetRealmInstance());
            set => _realmInstance = value;
        }

        protected ViewModelBase(IEventAggregator eventAggregator, INavigationService navigationService)
        {
            EventAggregator = eventAggregator;
            NavigationService = navigationService;

            Log.Verbose("Navigated to " + GetType().Name);
        }
    }

    public abstract class ViewModelConductorBase : Conductor<Screen>, IViewModelBase
    {
        private Realm _realmInstance;

        public string this[string index] => AppStrings.ResourceManager.GetString(index);
        public IEventAggregator EventAggregator { get; }
        public INavigationService NavigationService { get; }

        public Realm RealmInstance
        {
            get => _realmInstance ?? (_realmInstance = RealmHelpers.GetRealmInstance());
            set => _realmInstance = value;
        }

        protected ViewModelConductorBase(IEventAggregator eventAggregator, INavigationService navigationService)
        {
            EventAggregator = eventAggregator;
            NavigationService = navigationService;

            Log.Verbose("Navigated to " + GetType().Name);
            NavigationService.NavigationRequested += OnNavigationRequested;
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
