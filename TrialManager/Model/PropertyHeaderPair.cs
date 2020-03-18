using Stylet;

namespace TrialManager.Model
{
    /// <summary>
    /// Used to pair up a <see cref="MappedProperty"/> and a string header from a CSV file
    /// </summary>
    public class PropertyHeaderPair : PropertyChangedBase
    {
        private string _dataFileProperty;

        public MappedProperty MappedProperty { get; }
        public OptionalMappedProperty OptionalMappedProperty { get; }
        public bool IsOptional { get; }

        public string DataFileProperty
        {
            get => _dataFileProperty;
            set => SetAndNotify(ref _dataFileProperty, value);
        }

        public PropertyHeaderPair(MappedProperty property)
        {
            MappedProperty = property;
        }

        public PropertyHeaderPair(OptionalMappedProperty optionalMappedProperty)
        {
            OptionalMappedProperty = optionalMappedProperty;
            IsOptional = true;
        }

        public PropertyHeaderPair(MappedProperty mappedProperty, string dataFileProperty)
        {
            MappedProperty = mappedProperty;
            DataFileProperty = dataFileProperty;
        }

        public PropertyHeaderPair(OptionalMappedProperty optionalMappedProperty, string dataFileProperty)
        {
            OptionalMappedProperty = optionalMappedProperty;
            DataFileProperty = dataFileProperty;
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyHeaderPair mapping
                && mapping.MappedProperty.Equals(MappedProperty)
                && mapping.OptionalMappedProperty.Equals(OptionalMappedProperty)
                && mapping.DataFileProperty.Equals(DataFileProperty);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + MappedProperty.GetHashCode();
                hash = (hash * 7) + OptionalMappedProperty.GetHashCode();
                return (hash * 7) + DataFileProperty.GetHashCode();
            }
        }
    }
}
