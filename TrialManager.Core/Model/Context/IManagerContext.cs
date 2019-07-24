using Microsoft.EntityFrameworkCore;
using TrialManager.Core.Model.ContextModel;

namespace TrialManager.Core.Model.Context
{
    public interface IManagerContext
    {
        DbSet<Trialist> Trialists { get; set; }
    }
}
