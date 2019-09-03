using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.UI;
using Realms;
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

        private readonly Realm _realm;
        private readonly IIntraMessenger _messenger;

        private ObservableCollection<TrialistDrawEntry> _trialists;
        private bool _showProgress;
        private DateTime _trialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
        private string _trialName;
        private string _trialNotes;
        private string _trialAddress;
        private int _runsPerDay = 100;

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

        /// <summary>
        /// Gets or sets the address of the trial
        /// </summary>
        public string TrialAddress
        {
            get => _trialAddress;
            set => SetProperty(ref _trialAddress, value);
        }

        /// <summary>
        /// Gets or sets the number of runs per day in the trial
        /// </summary>
        public int RunsPerDay
        {
            get => _runsPerDay;
            set => SetProperty(ref _runsPerDay, value);
        }

        #endregion

        #region Commands

        public IMvxCommand CreateDrawCommand => new MvxCommand(OnCreateDraw);

        public IMvxCommand PrintDrawCommand => new MvxCommand(OnPrintDraw);

        #endregion

        public CreateDrawViewModel(IMvxNavigationService navigationService, IIntraMessenger messenger)
            : base (navigationService)
        {
            _realm = RealmHelpers.GetRealmInstance();
            _messenger = messenger;
        }

        public override void ViewAppearing()
        {
            OnCreateDraw();
            base.ViewAppearing();
        }

        private async void OnCreateDraw()
        {
            if (_realm.All<Trialist>().Any())
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

                foreach (Trialist element in _realm.All<Trialist>())
                {
                    foreach (Dog dog in element.Dogs)
                    {
                        TrialistDrawEntry entry = new TrialistDrawEntry(element, dog, count);
                        count++;
                        await AsyncDispatcher.ExecuteOnMainThreadAsync(() => Trialists.Add(entry)).ConfigureAwait(false);
                    }
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
