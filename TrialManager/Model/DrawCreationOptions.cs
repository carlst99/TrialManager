using TrialManager.Model.LocationDb;

namespace TrialManager.Model
{
    public class DrawCreationOptions
    {
        internal Gd2000Coordinate TrialLocation { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of runs that can be made in a day
        /// </summary>
        public int MaxRunsPerDay { get; set; }

        /// <summary>
        /// Gets or sets the address of the trial grounds, used for location sorting in the draw
        /// </summary>
        public string TrialAddress { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of other trialists' runs in between each of an individual trialist's runs
        /// </summary>
        public int MinRunSeparation { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of dogs from one trialist that can be run in one day
        /// </summary>
        public int MaxDogsPerDay { get; set; }

        /// <summary>
        /// Gets or sets the number of near-distance trialists to run before inserting far-distance trialists into the draw
        /// </summary>
        public int BufferRuns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether far-distance trialists should be run later on the first day (true), or to use the buffer-run protocol (false)
        /// </summary>
        public bool RunFurtherTrialistsLaterOnFirstDay { get; set; }

        public DrawCreationOptions()
        {
        }

        public DrawCreationOptions(string trialAddress, int maxRunsPerDay, int minRunSeparation, int maxDogsPerDay, int bufferRuns, bool runFurtherTrialistsLaterOnFirstDay)
        {
            TrialAddress = trialAddress;
            MaxRunsPerDay = maxRunsPerDay;
            MinRunSeparation = minRunSeparation;
            MaxDogsPerDay = maxDogsPerDay;
            BufferRuns = bufferRuns;
            RunFurtherTrialistsLaterOnFirstDay = runFurtherTrialistsLaterOnFirstDay;
        }
    }
}
