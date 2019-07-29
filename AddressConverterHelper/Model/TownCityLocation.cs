using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AddressConverterHelper.Model
{
    public class TownCityLocation : LocationBase
    {
        [Required]
        public List<SuburbLocalityLocation> Suburbs { get; set; } = new List<SuburbLocalityLocation>();

        public override void Prepare()
        {
            List<double> gd2000X = new List<double>();
            List<double> gd2000Y = new List<double>();

            foreach (SuburbLocalityLocation sLoc in Suburbs)
            {
                gd2000X.Add(sLoc.Gd2000X);
                gd2000Y.Add(sLoc.Gd2000Y);
            }

            Gd2000X = gd2000X.Average();
            Gd2000Y = gd2000Y.Average();
        }
    }
}
