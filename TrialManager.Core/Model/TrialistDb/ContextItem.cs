using Realms;

namespace TrialManager.Core.Model.TrialistDb
{
    public abstract class ContextItem : RealmObject
    {
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
