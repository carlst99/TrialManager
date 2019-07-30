using System.ComponentModel;

namespace TrialManager.Core.Model.TrialistDb
{
    public abstract class ContextItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
