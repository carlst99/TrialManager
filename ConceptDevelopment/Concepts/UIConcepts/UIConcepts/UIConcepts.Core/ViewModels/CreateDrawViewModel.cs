using MvvmCross.Navigation;
using System.Collections.Generic;
using System.Linq;
using UIConcepts.Core.Model.Context;
using UIConcepts.Core.Model.ContextModel;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("Create Draw")]
    public class CreateDrawViewModel : ViewModelBase
    {
        private readonly ManagerContext _managerContext;

        public List<Trialist> Trialists { get; }

        public CreateDrawViewModel(IMvxNavigationService navigationService, IManagerContext managerContext)
            : base (navigationService)
        {
            _managerContext = (ManagerContext)managerContext;

            if (_managerContext.Trialists.Any())
                Trialists = new List<Trialist>(_managerContext.Trialists.ToList());
            else
                navigationService.Navigate<DataDisplayViewModel>();
        }
    }
}
