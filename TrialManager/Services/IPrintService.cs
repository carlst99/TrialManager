using System.Collections.Generic;
using TrialManager.Model;

namespace TrialManager.Services
{
    public interface IPrintService
    {
        void Print(IEnumerable<TrialistDrawEntry> drawEntries, string title);
    }
}
