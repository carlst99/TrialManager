using MessagePack;
using Realms;
using System;
using System.Collections.Generic;
using TrialManager.Core.Model.LocationDb;

namespace TrialManager.Core.Model.TrialistDb
{
    public class Trialist : RealmObject, IContextItem
    {
        public static Trialist Default => new Trialist
        {
            Id = RealmHelpers.GetNextId<Trialist>(),
            FullName = "Full Name",
            Status = EntityStatus.Maiden,
            PhoneNumber = "012 345 6789",
            Email = "email@email.com",
            Address = "32 Hopeful Lane, Tamahere, Waikato",
            Dogs = { Dog.Default },
            Location = new Location()
        };

        private int StatusRaw { get; set; }
        private byte[] LocationRaw { get; set; }

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
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the trialist
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the email of the trialist
        /// </summary>
        public string Email { get; set; }

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
        /// Gets or sets the location of this <see cref="Trialist"/>
        /// </summary>
        public Location Location
        {
            get => MessagePackSerializer.Deserialize<Location>(LocationRaw);
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
        }

        #region Object Overrides

        public override string ToString()
        {
            return FullName;
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
        public bool IsContentEqual(object obj)
        {
            return obj is Trialist trialist
                && trialist.FullName.Equals(FullName)
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
                hash = (hash * 7) + FullName.GetHashCode();
                return (hash * 7) + Status.GetHashCode();
            }
        }
    }
}
