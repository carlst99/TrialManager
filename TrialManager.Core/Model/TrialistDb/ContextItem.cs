using Realms;

namespace TrialManager.Core.Model.TrialistDb
{
    public abstract class ContextItem : RealmObject
    {
        private int _id;

        /// <summary>
        /// Gets or sets the database ID
        /// </summary>
        [PrimaryKey]
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value, nameof(Id));
        }

        protected void SetProperty<T>(ref T container, T value, string propertyName)
        {
            if (container != null && !container.Equals(value))
            {
                container = value;
                RaisePropertyChanged(propertyName);
            }
            else if (container == null)
            {
                container = value;
                RaisePropertyChanged(propertyName);
            }
        }
    }
}
