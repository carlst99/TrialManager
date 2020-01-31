using System.ComponentModel;

namespace TrialManager.Model.TrialistDb
{
    public interface IContextItem : INotifyPropertyChanged
    {
        EntityStatus Status { get; set; }
        string Name { get; set; }
    }
}
