using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Core.Model;
using TrialManager.Core.Model.Messages;
using TrialManager.Core.Model.TrialistDb;
using TrialManager.Core.ViewModels.Base;

namespace TrialManager.Core.ViewModels
{
    [DisplayNavigation("Create Draw")]
    public class CreateDrawViewModel : ViewModelBase
    {
        #region Fields

        private readonly TrialistContext _managerContext;
        private readonly IIntraMessenger _messenger;

        private ObservableCollection<TrialistDrawEntry> _trialists;
        private bool _showProgress;
        private DateTime _trialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
        private string _trialName;
        private string _trialNotes;

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
        /// Gets or sets the date and time that the trial will start
        /// </summary>
        public DateTime TrialStartDate
        {
            get => _trialStartDate;
            set => SetProperty(ref _trialStartDate, value);
        }

        /// <summary>
        /// Gets or sets the name of the trial
        /// </summary>
        public string TrialName
        {
            get => _trialName;
            set => SetProperty(ref _trialName, value);
        }

        /// <summary>
        /// Gets or sets the trial notes
        /// </summary>
        public string TrialNotes
        {
            get => _trialNotes;
            set => SetProperty(ref _trialNotes, value);
        }

        #endregion

        #region Commands

        public IMvxCommand CreateDrawCommand => new MvxCommand(OnCreateDraw);

        public IMvxCommand PrintDrawCommand => new MvxCommand(OnPrintDraw);

        #endregion

        public CreateDrawViewModel(IMvxNavigationService navigationService, ITrialistContext managerContext, IIntraMessenger messenger)
            : base (navigationService)
        {
            _managerContext = (TrialistContext)managerContext;
            _messenger = messenger;
        }

        public override void ViewAppearing()
        {
            OnCreateDraw();
            base.ViewAppearing();
        }

        private async void OnCreateDraw()
        {
            if (_managerContext.Trialists.Any())
            {
                await GenerateDraw().ConfigureAwait(false);
            }
            else
            {
                void ResultCallback(DialogMessage.DialogButton d)
                {
                    if ((d & DialogMessage.DialogButton.Yes) != 0)
                    {
                        NavigationService.Navigate<DataDisplayViewModel>();
                        _messenger.Send(new PageNavigationMessage
                        {
                            PageType = typeof(DataDisplayViewModel)
                        });
                    } else
                    {
                        _messenger.Send(new PageNavigationMessage
                        {
                            PageType = typeof(CreateDrawViewModel)
                        });
                    }
                }

                DialogMessage dialogRequest = new DialogMessage
                {
                    Buttons = DialogMessage.DialogButton.Yes | DialogMessage.DialogButton.No,
                    Title = "No data found",
                    Content = "Do you wish to import some data?",
                    Callback = ResultCallback
                };
                _messenger.Send(dialogRequest);
            }
        }

        private async Task GenerateDraw()
        {
            ShowProgress = true;
            Trialists = new ObservableCollection<TrialistDrawEntry>();

            await Task.Factory.StartNew(async () =>
            {
                int count = 1;

                foreach (Trialist element in _managerContext.Trialists.ToList())
                {
                    TrialistDrawEntry entry = new TrialistDrawEntry(element, element.Dogs.First(), count);
                    count++;
                    await AsyncDispatcher.ExecuteOnMainThreadAsync(() => Trialists.Add(entry)).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            ShowProgress = false;
        }

        private void OnPrintDraw()
        {
            throw new NotImplementedException();
        }
    }
}
