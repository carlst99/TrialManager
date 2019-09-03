using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrialManager.Core.Model.LocationDb;

namespace TrialManager.Core.Model.TrialistDb
{
    public class Trialist : ContextItem
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

        #region Fields

        private string _fullName;
        private string _phoneNumber;
        private string _email;
        private string _address;
        private EntityStatus _status;
        private Location _location;
        private DateTimeOffset _preferredDay;
        private Trialist _travellingPartner;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the full name of the trialist
        /// </summary>
        [Required]
        [Indexed]
        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value, nameof(FullName));
        }

        /// <summary>
        /// Gets or sets the phone number of the trialist
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value, nameof(PhoneNumber));
        }

        /// <summary>
        /// Gets or sets the email of the trialist
        /// </summary>
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value, nameof(Email));
        }

        /// <summary>
        /// Gets or sets the home address of the trialist
        /// </summary>
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value, nameof(Address));
        }

        /// <summary>
        /// Gets or sets the status of the trialist
        /// </summary>
        [Required]
        public EntityStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        /// <summary>
        /// Gets or sets the dogs that belong to this <see cref="Trialist"/>
        /// </summary>
        [Required]
        public IList<Dog> Dogs { get; }

        /// <summary>
        /// Gets or sets the location of this <see cref="Trialist"/>
        /// </summary>
        public Location Location
        {
            get => _location;
            set => SetProperty(ref _location, value, nameof(Location));
        }

        /// <summary>
        /// Gets or sets the preferred run day
        /// </summary>
        public DateTimeOffset PreferredDay
        {
            get => _preferredDay;
            set => SetProperty(ref _preferredDay, value, nameof(PreferredDay));
        }

        /// <summary>
        /// Gets or sets the travelling partner of this trialist
        /// </summary>
        public Trialist TravellingPartner
        {
            get => _travellingPartner;
            set => SetProperty(ref _travellingPartner, value, nameof(TravellingPartner));
        }

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
                && trialist.Address.Equals(Address)
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
                hash = (hash * 7) + Status.GetHashCode();
                return (hash * 7) + Address.GetHashCode();
            }
        }
    }
}
