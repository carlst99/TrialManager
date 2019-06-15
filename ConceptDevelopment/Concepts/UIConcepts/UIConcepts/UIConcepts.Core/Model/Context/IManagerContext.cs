using Microsoft.EntityFrameworkCore;
using UIConcepts.Core.Model.ContextModel;

namespace UIConcepts.Core.Model.Context
{
    public interface IManagerContext
    {
        DbSet<Trialist> Trialists { get; set; }
    }
}
