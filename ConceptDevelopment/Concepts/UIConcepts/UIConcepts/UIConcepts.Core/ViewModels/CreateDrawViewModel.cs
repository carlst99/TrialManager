﻿using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIConcepts.Core.Model.Context;
using UIConcepts.Core.Model.ContextModel;
using UIConcepts.Core.Model.Messages;
using UIConcepts.Core.ViewModels.Base;

namespace UIConcepts.Core.ViewModels
{
    [DisplayNavigation("Create Draw")]
    public class CreateDrawViewModel : ViewModelBase
    {
        #region Fields

        private readonly ManagerContext _managerContext;
        private readonly IIntraMessenger _messenger;

        private List<Trialist> _trialists;
        private bool _showProgress;
        private TimeSpan _timePerRun;
        private DateTime _trialStartDate = DateTime.Now;
        private DateTime _trialEndDate = DateTime.Now.AddDays(1);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of trialists to use in the draw
        /// </summary>
        public List<Trialist> Trialists
        {
            get => _trialists;
            private set => SetProperty(ref _trialists, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the loading indicator should be shown
        /// </summary>
        public bool ShowProgress
        {
            get => _showProgress;
            set => SetProperty(ref _showProgress, value);
        }

        /// <summary>
        /// Gets or sets a value indicating the approximate length of each run
        /// </summary>
        public TimeSpan TimePerRun
        {
            get => _timePerRun;
            set => SetProperty(ref _timePerRun, value);
        }

        /// <summary>
        /// Gets or sets the date and time that the trial will start
        /// </summary>
        public DateTime TrialStartDate
        {
            get => _trialStartDate;
            set => SetProperty(ref _trialStartDate, value);
        }

        /// <summary>
        /// Gets or sets the date and time that the trial will end
        /// </summary>
        public DateTime TrialEndDate
        {
            get => _trialEndDate;
            set => SetProperty(ref _trialEndDate, value);
        }

        #endregion

        #region Commands

        public IMvxCommand CreateDrawCommand => new MvxCommand(CreateDraw);

        public IMvxCommand ResetTrialDatesCommand => new MvxCommand(() =>
        {
            TrialStartDate = DateTime.Now;
            TrialEndDate = DateTime.Now.AddDays(1);
        });

        #endregion

        public CreateDrawViewModel(IMvxNavigationService navigationService, IManagerContext managerContext, IIntraMessenger messenger)
            : base (navigationService)
        {
            _managerContext = (ManagerContext)managerContext;
            _messenger = messenger;
        }

        private void CreateDraw()
        {
            ShowProgress = true;

            if (_managerContext.Trialists.Any())
            {
                Trialists = new List<Trialist>(_managerContext.Trialists.ToList());
            }
            else
            {
                void ResultCallback(DialogAction d)
                {
                    if (d.HasFlag(DialogAction.Yes))
                        NavigationService.Navigate<DataDisplayViewModel>();
                }

                MessageDialogMessage dialogRequest = new MessageDialogMessage
                {
                    Actions = DialogAction.Yes | DialogAction.No,
                    Title = "No data found",
                    Content = "Do you wish to import some data?",
                    Callback = ResultCallback
                };
                _messenger.Send(dialogRequest);
            }

            ShowProgress = false;
        }
    }
}
