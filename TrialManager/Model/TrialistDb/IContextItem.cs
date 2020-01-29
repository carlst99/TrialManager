namespace TrialManager.Model.TrialistDb
{
    public interface IContextItem
    {
        int Id { get; set; }
        EntityStatus Status { get; set; }
        string Name { get; set; }
    }
}
