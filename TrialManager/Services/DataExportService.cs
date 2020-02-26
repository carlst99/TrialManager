using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using TrialManager.Model;
using TrialManager.Model.Csv;

namespace TrialManager.Services
{
    public class DataExportService : IDataExportService
    {
        public async Task ExportAsCsv(IEnumerable<TrialistDrawEntry> draw, string outputPath)
        {
            using StreamWriter sw = new StreamWriter(outputPath);
            using CsvWriter cw = new CsvWriter(sw, CultureInfo.CurrentCulture);
            cw.Configuration.RegisterClassMap<TrialistDrawEntryMapping>();
            await cw.WriteRecordsAsync(draw);
        }

        public async Task ExportAsPdf(IEnumerable<TrialistDrawEntry> draw, string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}
