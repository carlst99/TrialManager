﻿using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrialManager.Core.Model.ContextModel
{
    public class Trialist : ContextItem
    {
        public static Trialist Default => new Trialist
        {
            Surname = "Surname",
            FirstName = "FirstName",
            Status = EntityStatus.Maiden,
            PhoneNumber = "012 345 6789",
            Email = "email@email.com",
            Dogs = { Dog.Default }
        };

        #region Fields

        private int _trialistId;
        private string _firstName;
        private string _surname;
        private string _phoneNumber;
        private string _email;
        private EntityStatus _status;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the database ID of the trialist
        /// </summary>
        public int TrialistId
        {
            get => _trialistId;
            set => SetProperty(ref _trialistId, value, nameof(TrialistId));
        }

        /// <summary>
        /// Gets or sets the first name of the trialist
        /// </summary>
        [Required]
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value, nameof(FirstName));
        }

        /// <summary>
        /// Gets or sets the surname of the trialist
        /// </summary>
        [Required]
        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value, nameof(Surname));
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
        /// Gets or sets the status of the trialist
        /// </summary>
        [Required]
        public EntityStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        [NotMapped]
        public string FullName => GetFullName();

        /// <summary>
        /// Gets or sets the dogs that belong to this <see cref="Trialist"/>
        /// </summary>
        [Required]
        public ObservableCollection<Dog> Dogs { get; set; } = new ObservableCollection<Dog>();

        #endregion

        /// <summary>
        /// Removes a dog, ensuring that one still remains in the list
        /// </summary>
        /// <param name="dog">The dog to remove</param>
        public void SafeRemoveDog(Dog dog)
        {
            Dogs.Remove(dog);
            if (Dogs.Count <= 0)
                Dogs.Add(Dog.Default);
        }

        /// <summary>
        /// Gets the full name of the <see cref="Trialist"/>
        /// </summary>
        /// <returns></returns>
        public string GetFullName() => FirstName + " " + Surname;

        #region Object Overrides

        public override string ToString()
        {
            return FirstName + " " + Surname;
        }

        public override bool Equals(object obj)
        {
            return obj is Trialist trialist
                && trialist.FirstName.Equals(FirstName)
                && trialist.Surname.Equals(Surname)
                && trialist.TrialistId.Equals(TrialistId);
        }

        public override int GetHashCode()
        {
            const int hash = 13;
            return (hash * 7) + TrialistId;
        }

        #endregion
    }
}