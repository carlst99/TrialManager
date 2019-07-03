using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UIConcepts.Core.Model;
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

        private ObservableCollection<TrialistDrawEntry> _trialists;
        private bool _showProgress;
        private TimeSpan _timePerRun = new TimeSpan(0, 45, 0);
        private DateTime _trialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
        private DateTime _trialEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of trialists to use in the draw
        /// </summary>
        public ObservableCollection<TrialistDrawEntry> Trialists
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
        public DateTime TimePerRun
        {
            get => DateTime.MinValue + _timePerRun;
            set => SetProperty(ref _timePerRun, value - DateTime.MinValue);
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

        public IMvxCommand CreateDrawCommand => new MvxCommand(OnCreateDraw);

        public IMvxCommand ResetTrialDatesCommand => new MvxCommand(() =>
        {
            TrialStartDate = DateTime.Now;
            TrialEndDate = DateTime.Now.AddDays(1).AddHours(6);
        });

        public IMvxCommand PrintDrawCommand => new MvxCommand(OnPrintDraw);

        #endregion

        public CreateDrawViewModel(IMvxNavigationService navigationService, IManagerContext managerContext, IIntraMessenger messenger)
            : base (navigationService)
        {
            _managerContext = (ManagerContext)managerContext;
            _messenger = messenger;
            OnCreateDraw();
        }

        private async void OnCreateDraw()
        {
            ShowProgress = true;

            if (_managerContext.Trialists.Any())
            {
                await GenerateDraw().ConfigureAwait(false);
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

        private async Task GenerateDraw()
        {
            Trialists = new ObservableCollection<TrialistDrawEntry>();

            await Task.Factory.StartNew(async () =>
            {
                DateTime startTime = TrialStartDate;

                foreach (Trialist element in _managerContext.Trialists.ToList())
                {
                    TrialistDrawEntry entry = new TrialistDrawEntry(element, element.Dogs.First(), startTime);
                    startTime = startTime.AddHours(_timePerRun.Hours);
                    startTime = startTime.AddMinutes(_timePerRun.Minutes);
                    await AsyncDispatcher.ExecuteOnMainThreadAsync(() => Trialists.Add(entry)).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }

        private void OnPrintDraw()
        {
            throw new NotImplementedException();
        }
    }
}
