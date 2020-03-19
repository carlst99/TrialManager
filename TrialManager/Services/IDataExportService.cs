using System.Collections.Generic;
using System.Threading.Tasks;
using TrialManager.Model.Draw;

namespace TrialManager.Services
{
    public interface IDataExportService
    {
        Task ExportAsCsv(IEnumerable<TrialistDrawEntry> draw, string outputPath);
        Task ExportAsTxt(IEnumerable<TrialistDrawEntry> draw, string outputPath);
    }
}
