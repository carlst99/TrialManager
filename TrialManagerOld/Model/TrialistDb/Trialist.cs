using Stylet;
using System;
using System.Linq;
using TrialManager.Model.LocationDb;

namespace TrialManager.Model.TrialistDb
{
    public class Trialist : ContextItem
    {
        public static Trialist Default => new Trialist
        {
            Name = "Full Name",
            Status = "Maiden",
            Address = "32 Hopeful Lane, Testing Grounds, Waitomo",
            Dogs = { Dog.Default },
            CoordinatePoint = new Gd2000Coordinate()
        };

        #region Fields

        private string _address;
        private BindableCollection<Dog> _dogs;
        private Gd2000Coordinate _coordinatePoint;
        private DateTime _preferredDay;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the home address of the trialist
        /// </summary>
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        /// <summary>
        /// Gets or sets the dogs that belong to this <see cref="Trialist"/>
        /// </summary>
        public BindableCollection<Dog> Dogs
        {
            get => _dogs;
            set => SetProperty(ref _dogs, value);
        }

        /// <summary>
        /// Gets or sets the location of this <see cref="Trialist"/>
        /// </summary>
        public Gd2000Coordinate CoordinatePoint
        {
            get => _coordinatePoint;
            set => SetProperty(ref _coordinatePoint, value);
        }

        /// <summary>
        /// Gets or sets the preferred run day
        /// </summary>
        public DateTime PreferredDay
        {
            get => _preferredDay;
            set => SetProperty(ref _preferredDay, value);
        }

        #endregion

        public Trialist()
        {
            Dogs = new BindableCollection<Dog>();
        }

        #region Object Overrides

        public override bool Equals(object obj)
        {
            return obj is Trialist trialist
                && Equals(trialist);
        }

        public bool Equals(Trialist trialist)
        {
            return trialist.Name.Equals(Name)
                && trialist.Status.Equals(Status)
                && trialist.Dogs.SequenceEqual(Dogs);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Name.GetHashCode();
                hash = (hash * 7) + Status.GetHashCode();
                return (hash * 7) + Dogs.GetHashCode();
            }
        }
        #endregion
    }
}
