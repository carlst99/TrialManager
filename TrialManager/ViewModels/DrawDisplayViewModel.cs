using MaterialDesignThemes.Wpf;
using Stylet;
using System.Collections.Generic;
using System.Linq;
using TrialManager.Model.TrialistDb;
using TrialManager.Services;
using TrialManager.ViewModels.Base;

namespace TrialManager.ViewModels
{
    public class DrawDisplayViewModel : ViewModelBase
    {
        #region Fields

        private readonly ISnackbarMessageQueue _messageQueue;
        private BindableCollection<Trialist> _trialists;
        private string _trialAddress;

        #endregion

        #region Properties

        public BindableCollection<Trialist> Trialists
        {
            get => _trialists;
            set => SetAndNotify(ref _trialists, value);
        }

        public string TrialAddress
        {
            get => _trialAddress;
            set => SetAndNotify(ref _trialAddress, value);
        }

        #endregion

        public DrawDisplayViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            ISnackbarMessageQueue messageQueue)
            : base(eventAggregator, navigationService)
        {
            _messageQueue = messageQueue;
        }

        public override async void Prepare(object payload)
        {
            Trialists = new BindableCollection<Trialist>();
            await foreach (Trialist element in (IAsyncEnumerable<Trialist>)payload)
                Trialists.Add(element);
            Trialists = new BindableCollection<Trialist>(Trialists.OrderBy(t => t.Name).ToList());
        }
    }
}
