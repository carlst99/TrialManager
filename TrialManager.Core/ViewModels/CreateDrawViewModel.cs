using CsvHelper;
using IntraMessaging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Core.Model;
using TrialManager.Core.Model.Csv;
using TrialManager.Core.Model.Messages;
using TrialManager.Core.Model.TrialistDb;
using TrialManager.Core.Services;
using TrialManager.Core.ViewModels.Base;

namespace TrialManager.Core.ViewModels
{
    [DisplayNavigation("Create Draw")]
    public class CreateDrawViewModel : ViewModelBase
    {
        #region Fields

        private readonly Realm _realm;
        private readonly IIntraMessenger _messenger;
        private readonly IDrawCreationService _drawCreationService;

        private bool _showProgress;
        private DateTime _trialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
        private string _trialName;
        private string _trialNotes;
        private string _trialAddress;
        private int _runsPerDay = 100;
        private double _gridColumnSize = 250d;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of trialists to use in the draw
        /// </summary>
        public ObservableCollection<TrialistDrawEntry> RunsEntered { get; } = new ObservableCollection<TrialistDrawEntry>();

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

        /// <summary>
        /// Gets or sets the size of all the grid columns. Used to force auto-resize after draw is created
        /// </summary>
        public double GridColumnSize
        {
            get => _gridColumnSize;
            set => SetProperty(ref _gridColumnSize, value);
        }

        #endregion

        #region Commands

        public IMvxCommand CreateDrawCommand => new MvxCommand(OnCreateDraw);

        public IMvxCommand PrintDrawCommand => new MvxCommand(OnPrintDraw);

        public IMvxCommand ExportDrawCommand => new MvxCommand(OnExportDraw);

        #endregion

        public CreateDrawViewModel(IMvxNavigationService navigationService, IIntraMessenger messenger, IDrawCreationService drawCreationService)
            : base (navigationService)
        {
            _realm = RealmHelpers.GetRealmInstance();
            _messenger = messenger;
            _drawCreationService = drawCreationService;
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
            RunsEntered.Clear();
            GridColumnSize = 250d;

            //foreach (TrialistDrawEntry element in _drawCreationService.CreateDraw(RunsPerDay, TrialStartDate, TrialAddress))
            //    RunsEntered.Add(element);

            await Task.Factory.StartNew(async () =>
            {
                foreach (TrialistDrawEntry element in _drawCreationService.CreateDraw(RunsPerDay, TrialStartDate, TrialAddress))
                    await AsyncDispatcher.ExecuteOnMainThreadAsync(() => RunsEntered.Add(element)).ConfigureAwait(false);
            }).ConfigureAwait(true);

            ShowProgress = false;
            GridColumnSize = double.NaN;
        }

        private void OnExportDraw()
        {
            async void callback(FileDialogMessage.DialogResult result, string path)
            {
                if (result == FileDialogMessage.DialogResult.Failed)
                    return;

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    return;

                bool complete = await Task.Run(() =>
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(path))
                        using (CsvWriter cw = new CsvWriter(sw))
                        {
                            cw.Configuration.RegisterClassMap<TrialistDrawEntryMapping>();
                            cw.WriteRecords(RunsEntered);
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        App.LogError("Could not export draw", ex);
                        return false;
                    }
                }).ConfigureAwait(false);

                if (!complete)
                {
                    _messenger.Send(new DialogMessage
                    {
                        Title = "Export error",
                        Content = "Sorry, we couldn't export the draw. Please try again!\nIf you are trying to overwrite a draw file, make sure it is not open in another program",
                        Buttons = DialogMessage.DialogButton.Ok
                    });
                } else
                {
                    _messenger.Send(new DialogMessage
                    {
                        Title = "Export Complete",
                        Content = "The draw was successfully exported",
                        Buttons = DialogMessage.DialogButton.Ok
                    });
                }
            }

            FileDialogMessage fd = new FileDialogMessage
            {
                Title = "Export Location",
                Type = FileDialogMessage.DialogType.FileSave,
                Callback = callback
            };
            _messenger.Send(fd);
        }

        private void OnPrintDraw()
        {
            _messenger.Send(new DialogMessage
            {
                Title = "Sorry!",
                Content = "The print functionality has not yet been implemented. Please export the file instead, and print the exported copy"
            });
        }
    }
}
