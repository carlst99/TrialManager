using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Realms;
using System;
using TrialManager.Core.Model;
using TrialManager.Core.ViewModels.Base;

namespace TrialManager.Core.ViewModels
{
    public class DataImportViewModel : ViewModelBase<Tuple<string, bool>>
    {
        #region Fields

        private readonly Realm _realm;
        private readonly IIntraMessenger _messenger;

        private bool _showProgress;
        private bool _mergeData;
        private string _filePath;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the loading indicator should be shown
        /// </summary>
        public bool ShowProgress
        {
            get => _showProgress;
            set => SetProperty(ref _showProgress, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether imported data should be merged with the existing dataset
        /// </summary>
        public bool MergeData
        {
            get => _mergeData;
            set => SetProperty(ref _mergeData, value);
        }

        /// <summary>
        /// Gets or sets the path to the new data file
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Invoked when the user cancels the import process
        /// </summary>
        public IMvxCommand CancelImportCommand => new MvxCommand(() => NavigationService.Navigate<DataDisplayViewModel>());

        #endregion

        public DataImportViewModel(IMvxNavigationService navigationService, IIntraMessenger messenger)
            : base (navigationService)
        {
            _realm = RealmHelpers.GetRealmInstance();
            _messenger = messenger;
        }

        public override void Prepare(Tuple<string, bool> parameter)
        {
            FilePath = parameter.Item1;
            MergeData = parameter.Item2;
        }
    }
}
