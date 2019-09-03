using MessagePack;
using Realms;

namespace TrialManager.Core.Model.LocationDb
{
    public class SuburbLocalityLocation : RealmObject, ILocation
    {
        [Required]
        private byte[] LocationRaw { get; set; }

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
        public Location Location
        {
            get => MessagePackSerializer.Deserialize<Location>(LocationRaw);
            set => LocationRaw = MessagePackSerializer.Serialize(value);
        }

        public string TownCityName { get; set; }

        [Ignored]
        public string FullName => string.IsNullOrEmpty(TownCityName) ? Name : Name + ", " + TownCityName;

        #endregion

        #region Object overrides

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            return obj is ILocation lb
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
