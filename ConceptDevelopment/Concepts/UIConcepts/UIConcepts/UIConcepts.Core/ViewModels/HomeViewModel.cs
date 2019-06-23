using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIConcepts.Core.Model.Messages;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    public class HomeViewModel : ViewModelBase, IMasterPresentationViewModel
    {
        #region Fields

        private readonly IIntraMessenger _messagingService;
        private bool _drawerStatus;
        private object _detailView;

        #endregion

        #region Commands

        public IMvxCommand OnNavigateRequestedCommand => new MvxCommand<Type>(OnNavigateRequested);

        #endregion

        #region Properties

        public Dictionary<string, Type> NavigatableViewModels { get; }

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

        public HomeViewModel(IMvxNavigationService navigationService, IIntraMessenger messagingService)
            : base (navigationService)
        {
            NavigatableViewModels = new Dictionary<string, Type>();
            _messagingService = messagingService;
            _messagingService.Subscribe(OnMessageReceived);
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

        private void OnMessageReceived(IMessage message)
        {
            if (message is MessageDialogMessage mdMessage)
            {
                throw new NotImplementedException();
            }
        }
    }
}
