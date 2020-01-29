using Stylet;
using TrialManager.Services;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public class ShellViewModel : ViewModelConductorBase
    {
        public ShellViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            DataImportViewModel dataImportViewModel)
            : base (eventAggregator, navigationService)
        {
            ActiveItem = dataImportViewModel;
        }
    }
}
