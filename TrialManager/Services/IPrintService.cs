using System.Collections.Generic;
using TrialManager.Model.Draw;

namespace TrialManager.Services
{
    public interface IPrintService
    {
        bool Print(IEnumerable<TrialistDrawEntry> drawEntries, string title);
    }
}
