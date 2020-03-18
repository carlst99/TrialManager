using System.Collections.Generic;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Model.Draw
{
    internal class PreferredDayEqualityComparer : IEqualityComparer<Trialist>
    {
        public bool Equals(Trialist x, Trialist y)
        {
            return x.PreferredDay.Equals(y.PreferredDay);
        }

        public int GetHashCode(Trialist obj)
        {
            return obj.PreferredDay.GetHashCode();
        }
    }
}
