using MessagePack;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using TrialManager.Model.LocationDb;

namespace TrialManager.Model.TrialistDb
{
    public class Trialist : RealmObject, IContextItem
    {
        public static Trialist Default => new Trialist
        {
            Id = RealmHelpers.GetNextId<Trialist>(),
            Name = "Full Name",
            Status = EntityStatus.Maiden,
            Address = "32 Hopeful Lane, Tamahere, Waikato",
            Dogs = { Dog.Default },
            Location = new Gd2000Coordinate()
        };

        #region Fields

        private int StatusRaw { get; set; }
        private byte[] LocationRaw { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the database ID
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the trialist
        /// </summary>
        [Required]
        [Indexed]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the home address of the trialist
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the status of the trialist
        /// </summary>
        public EntityStatus Status
        {
            get => (EntityStatus)StatusRaw;
            set => StatusRaw = (int)value;
        }

        /// <summary>
        /// Gets or sets the dogs that belong to this <see cref="Trialist"/>
        /// </summary>
        public IList<Dog> Dogs { get; }

        /// <summary>
        /// Gets a list of dogs that is safe to use in UI binding
        /// </summary>
        public List<Dog> UISafeDogs => Dogs.ToList();

        /// <summary>
        /// Gets or sets the location of this <see cref="Trialist"/>
        /// </summary>
        public Gd2000Coordinate Location
        {
            get => MessagePackSerializer.Deserialize<Gd2000Coordinate>(LocationRaw);
            set => LocationRaw = MessagePackSerializer.Serialize(value);
        }

        /// <summary>
        /// Gets or sets the preferred run day
        /// </summary>
        public DateTimeOffset PreferredDay { get; set; }

        /// <summary>
        /// Gets or sets the travelling partner of this trialist
        /// </summary>
        public Trialist TravellingPartner { get; set; }

        #endregion

        /// <summary>
        /// Removes a dog, ensuring that one still remains in the list
        /// </summary>
        /// <param name="dog">The dog to remove</param>
        public void SafeRemoveDog(Dog dog)
        {
            Dogs.Remove(dog);
            if (Dogs.Count == 0)
                Dogs.Add(Dog.Default);
            RaisePropertyChanged(nameof(UISafeDogs));
        }

        public void SafeAddDog(Dog dog)
        {
            Dogs.Add(dog);
            RaisePropertyChanged(nameof(UISafeDogs));
        }

        #region Object Overrides

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is Trialist trialist
                && trialist.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            const int hash = 13;
            return (hash * 7) + Id;
        }
        #endregion

        /// <summary>
        /// Checks for equality at a content level, unlike <see cref="Equals(object)"/> which checks at DB level
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsContentEqual(Trialist trialist)
        {
            return trialist.Name.Equals(Name)
                && trialist.Status.Equals(Status);
        }

        /// <summary>
        /// Gets a hashcode based on the content of this <see cref="Trialist"/>
        /// </summary>
        /// <returns></returns>
        public int GetContentHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Name.GetHashCode();
                return (hash * 7) + Status.GetHashCode();
            }
        }
    }
}
