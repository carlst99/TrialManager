using System.Collections.Generic;

namespace AddressConverterHelper.Model
{
    public class TownCityLocation : LocationBase
    {
        public List<SuburbLocalityLocation> Suburbs { get; set; }
    }
}
