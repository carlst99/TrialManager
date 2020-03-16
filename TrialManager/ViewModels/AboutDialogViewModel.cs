using Stylet;
using System.Reflection;
using TrialManager.Services;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public class AboutDialogViewModel : ViewModelBase
    {
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString(3);

        public AboutDialogViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
            : base (eventAggregator, navigationService)
        {
        }
    }
}
