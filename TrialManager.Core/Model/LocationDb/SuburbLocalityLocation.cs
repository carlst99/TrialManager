using System.ComponentModel.DataAnnotations.Schema;

namespace TrialManager.Core.Model.LocationDb
{
    public class SuburbLocalityLocation : LocationBase
    {
        public string TownCityName { get; set; }

        [NotMapped]
        public string FullName => Name + ", " + TownCityName;
    }
}
