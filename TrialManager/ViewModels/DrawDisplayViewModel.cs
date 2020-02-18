using MaterialDesignThemes.Wpf;
using Stylet;
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

        #endregion

        #region Properties

        public BindableCollection<Trialist> Trialists
        {
            get => _trialists;
            set => SetAndNotify(ref _trialists, value);
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

        public override void Prepare(object payload)
        {
            Trialists = (BindableCollection<Trialist>)payload;
        }
    }
}
