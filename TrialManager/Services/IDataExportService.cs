using System.Collections.Generic;
using System.Threading.Tasks;
using TrialManager.Model;

namespace TrialManager.Services
{
    public interface IDataExportService
    {
        public Task ExportAsCsv(IEnumerable<TrialistDrawEntry> draw, string outputPath);
        public Task ExportAsTxt(IEnumerable<TrialistDrawEntry> draw, string outputPath);
    }
}
