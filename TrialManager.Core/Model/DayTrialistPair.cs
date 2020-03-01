﻿using System;
using System.Collections.Generic;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model
{
    public class DayTrialistPair
    {
        public readonly DateTimeOffset Day;
        public IEnumerable<Trialist> Trialists { get; set; }

        public DayTrialistPair(DateTimeOffset day)
            : this (day, null) { }
        public DayTrialistPair(DateTimeOffset day, IEnumerable<Trialist> trialists)
        {
            Day = day;
            Trialists = trialists;
        }
    }
}