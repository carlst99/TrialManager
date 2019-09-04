using System;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model
{
    public class TrialistDrawEntry
    {
        public string TrialistName { get; set; }
        public string CompetingDogName { get; set; }
        public int RunNumber { get; set; }

        public DateTime Day { get; set; }

        public TrialistDrawEntry(Trialist trialist, Dog dog, int runNumber, DateTime day)
        {
            if (!trialist.Dogs.Contains(dog))
                throw new ArgumentException("This trialist does not own the dog " + dog.ToString());

            TrialistName = trialist.FullName;
            CompetingDogName = dog.Name;
            RunNumber = runNumber;
            Day = day;
        }

        public TrialistDrawEntry(string trialistName, string dogName, int runNumber)
        {
            TrialistName = trialistName;
            CompetingDogName = dogName;
            RunNumber = runNumber;
        }
    }
}
