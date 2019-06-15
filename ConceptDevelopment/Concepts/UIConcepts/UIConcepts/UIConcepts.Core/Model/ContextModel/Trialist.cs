using System.Collections.ObjectModel;

namespace UIConcepts.Core.Model.ContextModel
{
    public class Trialist : ContextItem
    {
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
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value, nameof(FirstName));
        }

        /// <summary>
        /// Gets or sets the surname of the trialist
        /// </summary>
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
        public EntityStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        public ObservableCollection<Dog> Dogs { get; set; } = new ObservableCollection<Dog>();

        #endregion

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
                && trialist.PhoneNumber.Equals(PhoneNumber);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + FirstName.GetHashCode();
            hash = (hash * 7) + Surname.GetHashCode();
            return (hash * 7) + PhoneNumber.GetHashCode();
        }

        public static bool operator ==(Trialist one, Trialist two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Trialist one, Trialist two)
        {
            return !one.Equals(two);
        }

        #endregion
    }
}
