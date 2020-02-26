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
            await cw.WriteRecordsAsync(draw).ConfigureAwait(false);
        }

        public async Task ExportAsTxt(IEnumerable<TrialistDrawEntry> draw, string outputPath)
        {
            using StreamWriter sw = new StreamWriter(outputPath);
            await sw.WriteLineAsync("RUN NUMBER || NAME, STATUS || DOG NAME, DOG STATUS").ConfigureAwait(false);
            foreach (TrialistDrawEntry entry in draw)
            {
                string line = entry.RunNumber + " || ";
                line += entry.TrialistName + ", " + entry.TrialistStatus + " || ";
                line += entry.CompetingDogName + ", " + entry.CompetingDogStatus;
                await sw.WriteLineAsync(line).ConfigureAwait(false);
            }
        }
    }
}
