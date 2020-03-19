using System.Collections.Generic;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Model
{
    public struct DrawDisplayParams
    {
        public List<Trialist> Trialists { get; }
        public bool LocationSortingEnabled { get; }

        public DrawDisplayParams(List<Trialist> trialists, bool locationSortingEnabled)
        {
            Trialists = trialists;
            LocationSortingEnabled = locationSortingEnabled;
        }

        public override bool Equals(object obj)
        {
            return obj is DrawDisplayParams p
                && p.Trialists.Equals(Trialists)
                && p.LocationSortingEnabled.Equals(LocationSortingEnabled);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Trialists.GetHashCode();
                return (hash * 7) + LocationSortingEnabled.GetHashCode();
            }
        }

        public static bool operator ==(DrawDisplayParams left, DrawDisplayParams right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DrawDisplayParams left, DrawDisplayParams right)
        {
            return !(left == right);
        }
    }
}
