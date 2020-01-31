namespace TrialManager.Model.TrialistDb
{
    public interface IContextItem
    {
        EntityStatus Status { get; set; }
        string Name { get; set; }
    }
}
