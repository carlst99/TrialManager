using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LINZCsvConverter.Model
{
    public abstract class LocationBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the primary DB key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        [Name("suburb_locality")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the GD2000 X coordinate of this location
        /// </summary>
        [Name("gd2000_xcoord")]
        [NotMapped]
        public double Gd2000X
        {
            get => Location.Gd2000X;
            set => Location.Gd2000X = value;
        }

        /// <summary>
        /// Gets or sets the GD2000 Y coordinate of this location
        /// </summary>
        [Name("gd2000_ycoord")]
        [NotMapped]
        public double Gd2000Y
        {
            get => Location.Gd2000Y;
            set => Location.Gd2000Y = value;
        }

        /// <summary>
        /// Gets or sets the NZ Geodetic Datum 2000 (NZGD2000) coordinate point for this location
        /// </summary>
        [Required]
        public Location Location;

        #endregion

        public abstract void Prepare();

        #region Object overrides

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            return obj is LocationBase lb
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
