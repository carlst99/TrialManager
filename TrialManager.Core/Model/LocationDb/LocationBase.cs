using System.ComponentModel.DataAnnotations;

namespace TrialManager.Core.Model.LocationDb
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
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the GD2000 X coordinate of this location
        /// </summary>
        [Required]
        public double Gd2000X { get; set; }

        /// <summary>
        /// Gets or sets the GD2000 Y coordinate of this location
        /// </summary>
        [Required]
        public double Gd2000Y { get; set; }

        #endregion

        /// <summary>
        /// Returns the location of this object in a <see cref="Location"/> object
        /// </summary>
        /// <returns></returns>
        public Location GetLocation() => new Location(Gd2000X, Gd2000Y);

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
