using MessagePack;

namespace UIConcepts.Core.Model.ContextModel
{
    [MessagePackObject]
    public class Dog : ContextItem
    {
        public static Dog Default => new Dog
        {
            Name = "Dog",
            Status = EntityStatus.Maiden
        };

        #region Fields

        private int _dogId;
        private string _name;
        private EntityStatus _status;

        #endregion Fields

        #region Properties

        [Key(0)]
        public int DogId
        {
            get => _dogId;
            set => SetProperty(ref _dogId, value, nameof(DogId));
        }

        /// <summary>
        /// Gets or sets the name of the dog
        /// </summary>
        [Key(1)]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, nameof(Name));
        }

        /// <summary>
        /// Gets or sets the status of the dog
        /// </summary>
        [Key(2)]
        public EntityStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is Dog dog
                && dog.Name.Equals(Name)
                && dog.Status.Equals(Status);
        }

        public override int GetHashCode()
        {
            const int hash = 13;
            return (hash * 7) + DogId;
        }

        public static bool operator ==(Dog one, Dog two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Dog one, Dog two)
        {
            return !one.Equals(two);
        }

        #endregion
    }
}