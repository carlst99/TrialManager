using Microsoft.AppCenter;
using Realms;
using Stylet;
using System.Reflection;
using TrialManager.Model;
using TrialManager.Services;
using TrialManager.Utils;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public class AboutDialogViewModel : ViewModelBase
    {
        private readonly Realm _realmInstance;
        private readonly Preferences _preferences;

        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString(3);

        public bool IsDiagnosticsEnabled
        {
            get => _preferences.IsDiagnosticsEnabled;
            set
            {
                _realmInstance.Write(() => _preferences.IsDiagnosticsEnabled = value);
                AppCenter.SetEnabledAsync(value);
                NotifyOfPropertyChange(nameof(IsDiagnosticsEnabled));
            }
        }

        public AboutDialogViewModel(IEventAggregator eventAggregator, NavigationService navigationService)
            : base (eventAggregator, navigationService)
        {
            _realmInstance = RealmHelpers.GetRealmInstance();
            _preferences = RealmHelpers.GetUserPreferences(_realmInstance);
        }
    }
}
