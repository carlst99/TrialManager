﻿using System;
using TrialManager.Core.Model.ContextModel;

namespace TrialManager.Core.Model
{
    public class TrialistDrawEntry
    {
        public Trialist Trialist { get; set; }
        public Dog CompetingDog { get; set; }
        public int RunNumber { get; set; }

        public TrialistDrawEntry(Trialist trialist, Dog dog, int runNumber)
        {
            if (!trialist.Dogs.Contains(dog))
                throw new ArgumentException("This trialist does not own the dog " + dog.ToString());

            Trialist = trialist;
            CompetingDog = dog;
            RunNumber = runNumber;
        }
    }
}