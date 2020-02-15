using System;

namespace TrialManager.Model
{
    /// <summary>
    /// Used to pair up a preferred day string to a <see cref="DateTimeOffset"/> object
    /// </summary>
    public class PreferredDayDateTimePair
    {
        public string PreferredDay { get; set; }
        public DateTimeOffset Day { get; set; }

        public PreferredDayDateTimePair(string preferredDay)
        {
            PreferredDay = preferredDay;
            Day = DateTimeOffset.MinValue;
        }

        public PreferredDayDateTimePair(string preferredDay, DateTimeOffset day)
        {
            PreferredDay = preferredDay;
            Day = day;
        }

        public override bool Equals(object obj)
        {
            return obj is PreferredDayDateTimePair pair
                && pair.PreferredDay.Equals(PreferredDay)
                && pair.Day.Equals(Day);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + PreferredDay.GetHashCode();
                return (hash * 7) + Day.GetHashCode();
            }
        }
    }
}
