using System;
using UIConcepts.Core.Model.ContextModel;

namespace UIConcepts.Core.Model
{
    public class TrialistDrawEntry
    {
        public Trialist Trialist;
        public Dog CompetingDog;
        public DateTime StartAt;

        public TrialistDrawEntry(Trialist trialist, Dog dog, DateTime startAt)
        {
            if (!trialist.Dogs.Contains(dog))
                throw new ArgumentException("This trialist does not own the dog " + dog.ToString());

            Trialist = trialist;
            CompetingDog = dog;
            StartAt = startAt;
        }
    }
}
