using System.Collections.Generic;
using TrialManager.Model;

namespace TrialManager.Services
{
    public interface IPrintService
    {
        bool Print(IEnumerable<TrialistDrawEntry> drawEntries);
    }
}
