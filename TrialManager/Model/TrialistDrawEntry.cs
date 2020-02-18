using System;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Model
{
    public class TrialistDrawEntry
    {
        public string TrialistName { get; set; }
        public string TrialistStatus { get; set; }
        public string CompetingDogName { get; set; }
        public string CompetingDogStatus { get; set; }
        public int RunNumber { get; set; }

        public TrialistDrawEntry(Trialist trialist, Dog dog, int runNumber)
        {
            if (!trialist.Dogs.Contains(dog))
                throw new ArgumentException("This trialist does not own the dog " + dog.ToString());

            TrialistName = trialist.Name;
            TrialistStatus = trialist.Status;
            CompetingDogName = dog.Name;
            CompetingDogStatus = dog.Status;
            RunNumber = runNumber;
        }
    }
}
