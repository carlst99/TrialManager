using System.Collections.Generic;
using System.Threading.Tasks;
using TrialManager.Model;

namespace TrialManager.Services
{
    public interface IPrintService
    {
        Task<bool> Print(IEnumerable<TrialistDrawEntry> drawEntries, string title);
    }
}
