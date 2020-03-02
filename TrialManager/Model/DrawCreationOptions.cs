namespace TrialManager.Model
{
    public class DrawCreationOptions
    {
        public int MaxRunsPerDay { get; set; }
        public string TrialAddress { get; set; }
        public int MinRunSeparation { get; set; }
        public int MaxDogsPerDay { get; set; }
        public int BufferRuns { get; set; }

        public DrawCreationOptions()
        {
        }

        public DrawCreationOptions(string trialAddress, int maxRunsPerDay, int minRunSeparation, int maxDogsPerDay, int bufferRuns)
        {
            TrialAddress = trialAddress;
            MaxRunsPerDay = maxRunsPerDay;
            MinRunSeparation = minRunSeparation;
            MaxDogsPerDay = maxDogsPerDay;
            BufferRuns = bufferRuns;
        }
    }
}
