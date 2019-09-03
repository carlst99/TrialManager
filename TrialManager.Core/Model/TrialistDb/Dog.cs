using MessagePack;
using Realms;

namespace TrialManager.Core.Model.TrialistDb
{
    [MessagePackObject]
    public class Dog : RealmObject, IContextItem
    {
        public static Dog Default => new Dog
        {
            Id = RealmHelpers.GetNextId<Dog>(),
            Name = "Dog",
            Status = EntityStatus.Maiden
        };

        private int StatusRaw { get; set; }

        #region Properties

        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the dog
        /// </summary>
        [Key(1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status of the dog
        /// </summary>
        [Key(2)]
        public EntityStatus Status
        {
            get => (EntityStatus)StatusRaw;
            set => StatusRaw = (int)value;
        }

        #endregion

        #region Constructors

        public Dog() { }

        public Dog(string name, EntityStatus status)
        {
            Name = name;
            Status = status;
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
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Status.GetHashCode();
                return (hash * 7) + Name.GetHashCode();
            }
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