using Realms;

namespace TrialManager.Core.Model.LocationDb
{
    public abstract class LocationBase : RealmObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the primary DB key
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        [Required]
        [Indexed]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the NZ Geodetic Datum 2000 (NZGD2000) coordinate point for this location
        /// </summary>
        [Required]
        public Location Location { get; set; }

        #endregion

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
