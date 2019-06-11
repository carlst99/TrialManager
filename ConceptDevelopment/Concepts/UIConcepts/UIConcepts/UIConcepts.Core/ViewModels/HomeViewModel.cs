using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    public class HomeViewModel : ViewModelBase, IMasterPresentationViewModel
    {
        #region Fields

        private bool _drawerStatus;
        private object _detailView;

        public Dictionary<string, Type> NavigatableViewModels { get; }

        #endregion

        #region Commands

        public IMvxCommand OnNavigateRequestedCommand => new MvxCommand<Type>(OnNavigateRequested);

        #endregion

        #region Properties

        public bool IsDrawerOpen
        {
            get => _drawerStatus;
            set => SetProperty(ref _drawerStatus, value);
        }

        public object DetailView
        {
            get => _detailView;
            set => SetProperty(ref _detailView, value);
        }

        #endregion

        public HomeViewModel(IMvxNavigationService navigationService)
            : base (navigationService)
        {
            NavigatableViewModels = new Dictionary<string, Type>();
        }

        public override void ViewAppearing()
        {
            base.ViewCreated();
            QueryNavigatableTypes();
        }

        private void OnNavigateRequested(Type navigationItem)
        {
            NavigationService.Navigate(navigationItem);
            IsDrawerOpen = false;
        }

        private void QueryNavigatableTypes()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var types = from type in currentAssembly.GetTypes()
                        where Attribute.IsDefined(type, typeof(DisplayNavigationAttribute))
                        select type;
            foreach (Type type in types)
            {
                DisplayNavigationAttribute attribute = type.GetCustomAttribute<DisplayNavigationAttribute>();
                NavigatableViewModels.Add(attribute.DisplayName, type);
            }
            if (NavigatableViewModels.Count > 0)
                OnNavigateRequested(NavigatableViewModels.Values.First());
        }
    }
}
