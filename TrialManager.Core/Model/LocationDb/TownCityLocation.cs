using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrialManager.Core.Model.LocationDb
{
    public class TownCityLocation : LocationBase
    {
        [Required]
        public List<SuburbLocalityLocation> Suburbs { get; set; } = new List<SuburbLocalityLocation>();
    }
}
