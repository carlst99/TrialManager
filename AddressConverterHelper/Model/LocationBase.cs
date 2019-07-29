namespace AddressConverterHelper.Model
{
    public abstract class LocationBase
    {
        /// <summary>
        /// Gets or sets the primary DB key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the GD2000 X coordinate of this location
        /// </summary>
        public double Gd2000X { get; set; }

        /// <summary>
        /// Gets or sets the GD2000 Y coordinate of this location
        /// </summary>
        public double Gd2000Y { get; set; }

        public override string ToString() => Name;
    }
}
