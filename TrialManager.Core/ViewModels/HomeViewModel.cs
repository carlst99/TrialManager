﻿using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCrossExtensions.Wpf.Presenters.MasterDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TrialManager.Core.Model.Messages;
using TrialManager.Core.ViewModels.Base;

namespace TrialManager.Core.ViewModels
{
    public class HomeViewModel : ViewModelBase, IMasterPresentationViewModel
    {
        #region Fields

        private object _detailView;
        private int _selectedPageIndex;

        #endregion

        #region Commands

        public IMvxCommand OnNavigateRequestedCommand => new MvxCommand<Type>(OnNavigateRequested);

        #endregion

        #region Properties

        /// <summary>
        /// Gets a dictionary of navigatable viewmodel types, and their friendly name
        /// </summary>
        public Dictionary<Type, string> NavigatableViewModels { get; }

        /// <summary>
        /// Gets or sets the detail view of this <see cref="IMasterPresentationViewModel"/>
        /// </summary>
        public object DetailView
        {
            get => _detailView;
            set => SetProperty(ref _detailView, value);
        }

        /// <summary>
        /// Gets or sets the index of the selected page
        /// </summary>
        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set => SetProperty(ref _selectedPageIndex, value);
        }

        #endregion

        public HomeViewModel(IMvxNavigationService navigationService, IIntraMessenger messenger)
            : base (navigationService)
        {
            messenger.Subscribe(OnMessage, new Type[] { typeof(PageNavigationMessage) });
            NavigatableViewModels = new Dictionary<Type, string>();
        }

        public override Task Initialize()
        {
            QueryNavigatableTypes();
            return base.Initialize();
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            if (NavigatableViewModels.Count > 0)
                OnNavigateRequested(NavigatableViewModels.Keys.First());
        }

        private void OnNavigateRequested(Type navigationItem)
        {
            NavigationService.Navigate(navigationItem);
        }

        /// <summary>
        /// Obtains a list of viewmodel types that should be listed in the navigation pane
        /// </summary>
        private void QueryNavigatableTypes()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var types = from type in currentAssembly.GetTypes()
                        where Attribute.IsDefined(type, typeof(DisplayNavigationAttribute))
                        select type;
            foreach (Type type in types)
            {
                DisplayNavigationAttribute attribute = type.GetCustomAttribute<DisplayNavigationAttribute>();
                NavigatableViewModels.Add(type, attribute.DisplayName);
            }
        }

        private void OnMessage(IMessage message)
        {
            if (message is PageNavigationMessage pMessage)
            {
                SelectedPageIndex = NavigatableViewModels.Keys.ToList().IndexOf(pMessage.PageType);
            }
        }
    }
}
