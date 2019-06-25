using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIConcepts.Core.Model.Context;
using UIConcepts.Core.Model.ContextModel;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("Create Draw")]
    public class CreateDrawViewModel : ViewModelBase
    {
        private ManagerContext _managerContext;

        public List<Trialist> Trialists { get; }

        public CreateDrawViewModel(IMvxNavigationService navigationService, IManagerContext managerContext)
            : base (navigationService)
        {
            _managerContext = (ManagerContext)managerContext;

            if (_managerContext.Trialists.Any())
                Trialists = new List<Trialist>(_managerContext.Trialists.ToList());
            else
                Trialists = new List<Trialist>();
        }
    }
}
