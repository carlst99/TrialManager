using System;
using System.Collections.Generic;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Model.Draw
{
    public class DayTrialistPair
    {
        public readonly DateTime Day;
        public IEnumerable<Trialist> Trialists { get; set; }

        public DayTrialistPair(DateTime day)
            : this (day, null)
        {
        }

        public DayTrialistPair(DateTime day, IEnumerable<Trialist> trialists)
        {
            Day = day;
            Trialists = trialists;
        }
    }
}
