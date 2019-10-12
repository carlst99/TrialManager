using CsvHelper.Configuration.Attributes;
using MessagePack;
using Realms;
using System.Collections.Generic;
using System.Linq;

namespace LINZCsvConverter.Model
{
    public class SuburbLocalityLocation : RealmObject
    {
        [Required]
        private byte[] LocationRaw { get; set; }

        [Ignored]
        public List<double> XCollection { get; } = new List<double>();

        [Ignored]
        public List<double> YCollection { get; } = new List<double>();

        [Name("town_city")]
        public string TownCityName { get; set; }

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
        /// Gets or sets the GD2000 X coordinate of this location
        /// </summary>
        [Name("gd2000_xcoord")]
        [Ignored]
        public double Gd2000X { get; set; }

        /// <summary>
        /// Gets or sets the GD2000 Y coordinate of this location
        /// </summary>
        [Name("gd2000_ycoord")]
        [Ignored]
        public double Gd2000Y { get; set; }

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

        public void Merge(SuburbLocalityLocation obj)
        {
            XCollection.Add(obj.Gd2000X);
            YCollection.Add(obj.Gd2000Y);
        }

        public void Prepare()
        {
            if (XCollection.Count > 0)
                Gd2000X = XCollection.Average();

            if (YCollection.Count > 0)
                Gd2000Y = YCollection.Average();

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
            return obj is SuburbLocalityLocation lb
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
