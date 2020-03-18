using MessagePack;
using Stylet;
using System;
using TrialManager.Model.LocationDb;

namespace TrialManager.Model.Draw
{
    [MessagePackObject]
    public class DrawCreationOptions : PropertyChangedBase
    {
        private string _trialAddress;
        private int _maxRunsPerDay;
        private int _minRunSeparation;
        private int _maxDogsPerDay;
        private int _bufferRuns;
        private bool _runFurtherTrialistsLaterOnFirstDay;
        private DateTime _trialStartDate;

        internal Gd2000Coordinate TrialLocation { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of runs that can be made in a day
        /// </summary>
        [Key(0)]
        public int MaxRunsPerDay
        {
            get => _maxRunsPerDay;
            set => SetAndNotify(ref _maxRunsPerDay, value);
        }

        /// <summary>
        /// Gets or sets the address of the trial grounds, used for location sorting in the draw
        /// </summary>
        public string TrialAddress
        {
            get => _trialAddress;
            set => SetAndNotify(ref _trialAddress, value);
        }

        /// <summary>
        /// Gets or sets the minimum number of other trialists' runs in between each of an individual trialist's runs
        /// </summary>
        [Key(1)]
        public int MinRunSeparation
        {
            get => _minRunSeparation;
            set => SetAndNotify(ref _minRunSeparation, value);
        }

        /// <summary>
        /// Gets or sets the maximum number of dogs from one trialist that can be run in one day
        /// </summary>
        [Key(2)]
        public int MaxDogsPerDay
        {
            get => _maxDogsPerDay;
            set => SetAndNotify(ref _maxDogsPerDay, value);
        }

        /// <summary>
        /// Gets or sets the number of near-distance trialists to run before inserting far-distance trialists into the draw
        /// </summary>
        [Key(3)]
        public int BufferRuns
        {
            get => _bufferRuns;
            set => SetAndNotify(ref _bufferRuns, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether far-distance trialists should be run later on the first day (true), or to use the buffer-run protocol (false)
        /// </summary>
        [Key(4)]
        public bool RunFurtherTrialistsLaterOnFirstDay
        {
            get => _runFurtherTrialistsLaterOnFirstDay;
            set => SetAndNotify(ref _runFurtherTrialistsLaterOnFirstDay, value);
        }

        public DateTime TrialStartDate
        {
            get => _trialStartDate;
            set => SetAndNotify(ref _trialStartDate, value);
        }

        public event EventHandler OnOptionsChanged;

        public DrawCreationOptions()
        {
            MaxRunsPerDay = 100;
            MinRunSeparation = 3;
            MaxDogsPerDay = 5;
            BufferRuns = 10;
            RunFurtherTrialistsLaterOnFirstDay = false;
            TrialStartDate = DateTime.Now;
        }

        public DrawCreationOptions(string trialAddress, int maxRunsPerDay, int minRunSeparation, int maxDogsPerDay, int bufferRuns, bool runFurtherTrialistsLaterOnFirstDay, DateTime trialStartDate)
        {
            TrialAddress = trialAddress;
            MaxRunsPerDay = maxRunsPerDay;
            MinRunSeparation = minRunSeparation;
            MaxDogsPerDay = maxDogsPerDay;
            BufferRuns = bufferRuns;
            RunFurtherTrialistsLaterOnFirstDay = runFurtherTrialistsLaterOnFirstDay;
            TrialStartDate = trialStartDate;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            OnOptionsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
