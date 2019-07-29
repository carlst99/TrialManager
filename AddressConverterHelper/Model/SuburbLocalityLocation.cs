using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AddressConverterHelper.Model
{
    public class SuburbLocalityLocation : LocationBase
    {
        [NotMapped]
        public List<double> XCollection { get; } = new List<double>();

        [NotMapped]
        public List<double> YCollection { get; } = new List<double>();

        [Name("town_city")]
        public string TownCityName { get; set; }

        public void Merge(SuburbLocalityLocation obj)
        {
            XCollection.Add(obj.Gd2000X);
            YCollection.Add(obj.Gd2000Y);
        }

        public override void Prepare()
        {
            if (XCollection.Count > 0)
                Gd2000X = XCollection.Average();

            if (YCollection.Count > 0)
                Gd2000Y = YCollection.Average();
        }
    }
}
