﻿namespace TrialManager.Model
{
    /// <summary>
    /// Used to pair up a <see cref="MappedProperty"/> and a string header from a CSV file
    /// </summary>
    public class PropertyHeaderPair
    {
        public MappedProperty MappedProperty { get; set; }
        public string DataFileProperty { get; set; }

        public PropertyHeaderPair(MappedProperty property)
        {
            MappedProperty = property;
        }

        public PropertyHeaderPair(MappedProperty property, string dataFileProperty)
        {
            MappedProperty = property;
            DataFileProperty = dataFileProperty;
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyHeaderPair mapping
                && mapping.MappedProperty.Equals(MappedProperty)
                && mapping.DataFileProperty.Equals(DataFileProperty);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + MappedProperty.GetHashCode();
                return (hash * 7) + DataFileProperty.GetHashCode();
            }
        }
    }
}
