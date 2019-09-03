using Realms;
using System.Collections.Generic;

namespace TrialManager.Core.Model.LocationDb
{
    public class TownCityLocation : LocationBase
    {
        [Required]
        public IList<SuburbLocalityLocation> Suburbs { get; }
    }
}
