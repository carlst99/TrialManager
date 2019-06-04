using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System;
using System.Collections.Generic;

namespace UIConcepts.Core.ViewModels
{
    public class HomeViewModel : Base.ViewModelBase, IMasterPresentationViewModel
    {
        public HomeViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }

        #region Fields

        private bool _drawerStatus;
        private object _detailView;

        private readonly Dictionary<string, Type> _keyViewModelValuePairs = new Dictionary<string, Type>()
        {
            { "View/Edit Data", typeof(DataDisplayViewModel) }
        };

        #endregion

        #region Commands

        public IMvxCommand OnNavigateRequestedCommand => new MvxCommand(OnNavigateRequested);

        #endregion

        #region Properties

        public bool DrawerStatus
        {
            get => _drawerStatus;
            set => SetProperty(ref _drawerStatus, value);
        }

        public object DetailView
        {
            get => _detailView;
            set => SetProperty(ref _detailView, value);
        }

        public Dictionary<string, Type>.KeyCollection AvailableNavigationPages => _keyViewModelValuePairs.Keys;

        #endregion

        private void OnNavigateRequested()
        {
            throw new NotImplementedException();
        }
    }
}
