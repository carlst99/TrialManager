using Microsoft.EntityFrameworkCore;

namespace TrialManager.Core.Model.TrialistDb
{
    public interface ITrialistContext
    {
        DbSet<Trialist> Trialists { get; set; }
    }
}
