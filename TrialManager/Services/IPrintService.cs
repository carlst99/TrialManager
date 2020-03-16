using System.Collections.Generic;
using System.Windows.Controls;
using TrialManager.Model;

namespace TrialManager.Services
{
    public interface IPrintService
    {
        void Print(IEnumerable<TrialistDrawEntry> drawEntries, string title, DataGrid dg);
    }
}
