using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TrialManager.Model.TrialistDb
{
    public abstract class ContextItem : IContextItem
    {
        #region Fields

        private EntityStatus _status;
        private string _name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the status of this entity
        /// </summary>
        public EntityStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets or sets the full name of this entity
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when a property is changed on this <see cref="ContextItem"/>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void SetProperty<T>(ref T container, T value, [CallerMemberName]string propertyName = null)
        {
            if (container == null || !container.Equals(value))
            {
                container = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString() => Name;
    }
}
