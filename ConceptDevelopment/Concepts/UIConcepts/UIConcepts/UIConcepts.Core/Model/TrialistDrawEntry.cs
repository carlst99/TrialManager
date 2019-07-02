using System;
using UIConcepts.Core.Model.ContextModel;

namespace UIConcepts.Core.Model
{
    public class TrialistDrawEntry
    {
        public Trialist Trialist { get; set; }
        public Dog CompetingDog { get; set; }
        public DateTime RunStart { get; set; }

        public string RunStartStringFormatted => RunStart.ToString("ddd hh:mm tt");

        public TrialistDrawEntry(Trialist trialist, Dog dog, DateTime runStart)
        {
            if (!trialist.Dogs.Contains(dog))
                throw new ArgumentException("This trialist does not own the dog " + dog.ToString());

            Trialist = trialist;
            CompetingDog = dog;
            RunStart = runStart;
        }
    }
}
