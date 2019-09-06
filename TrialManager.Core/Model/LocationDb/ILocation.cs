using Realms;

namespace TrialManager.Core.Model.LocationDb
{
    public interface ILocation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the primary DB key
        /// </summary>
        [PrimaryKey]
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        [Required]
        [Indexed]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the NZ Geodetic Datum 2000 (NZGD2000) coordinate point for this location
        /// </summary>
        Location Location { get; set; }

        #endregion
    }
}
