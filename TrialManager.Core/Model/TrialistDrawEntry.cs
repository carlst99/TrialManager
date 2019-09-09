using System;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model
{
    public class TrialistDrawEntry
    {
        private const string SEPARATOR = " | ";

        public string TrialistName { get; set; }

        public string CompetingDogName { get; set; }
        public int RunNumber { get; set; }
        public DateTimeOffset Day { get; set; }

        public TrialistDrawEntry(Trialist trialist, Dog dog, int runNumber, DateTimeOffset day)
        {
            if (!trialist.Dogs.Contains(dog))
                throw new ArgumentException("This trialist does not own the dog " + dog.ToString());

            TrialistName = trialist.Name + SEPARATOR + trialist.Status;
            CompetingDogName = dog.Name + SEPARATOR + dog.Status;
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
