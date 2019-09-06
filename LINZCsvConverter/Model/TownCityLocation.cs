using CsvHelper.Configuration.Attributes;
using MessagePack;
using Realms;
using System.Collections.Generic;
using System.Linq;

namespace LINZCsvConverter.Model
{
    public class TownCityLocation : RealmObject
    {
        [Required]
        private byte[] LocationRaw { get; set; }

        public IList<SuburbLocalityLocation> Suburbs { get; }

        /// <summary>
        /// Gets or sets the primary DB key
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        [Name("suburb_locality")]
        [Required]
        [Indexed]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the NZ Geodetic Datum 2000 (NZGD2000) coordinate point for this location
        /// </summary>
        public Location Location
        {
            get
            {
                if (LocationRaw != null)
                    return MessagePackSerializer.Deserialize<Location>(LocationRaw);
                else
                    return new Location();
            }
            set => LocationRaw = MessagePackSerializer.Serialize(value);
        }

        public void Prepare()
        {
            List<double> gd2000X = new List<double>();
            List<double> gd2000Y = new List<double>();

            foreach (SuburbLocalityLocation sLoc in Suburbs)
            {
                gd2000X.Add(sLoc.Gd2000X);
                gd2000Y.Add(sLoc.Gd2000Y);
            }

            double Gd2000X = gd2000X.Average();
            double Gd2000Y = gd2000Y.Average();
            Location = new Location
            {
                Gd2000X = Gd2000X,
                Gd2000Y = Gd2000Y
            };
        }

        #region Object overrides

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            return obj is TownCityLocation lb
                && lb.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (13 * 7) + Name.GetHashCode();
            }
        }

        #endregion
    }
}
