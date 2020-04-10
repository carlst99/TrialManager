using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TrialManager.Services
{
    public class EventSeparatorService
    {
        private readonly ICsvImportService _importService;

        public EventSeparatorService(ICsvImportService importService)
        {
            _importService = importService;
        }

        public async Task Separate(string path, int headerIndex)
        {
            List<string> recordEvents = new List<string>();

            using (CsvReader reader = _importService.GetCsvReader(path))
            {
                await reader.ReadAsync().ConfigureAwait(false); // Make sure that we don't read the header record
                while (await reader.ReadAsync().ConfigureAwait(false))
                    recordEvents.Add(reader.GetField(headerIndex));
            }

            Dictionary<string, StreamWriter> eventFileWriters = new Dictionary<string, StreamWriter>();
            string fileName = path.Replace(".csv", string.Empty);

            using (StreamReader sr = new StreamReader(path))
            {
                string headerRecord = await sr.ReadLineAsync().ConfigureAwait(false);

                foreach (string rEvent in recordEvents)
                {
                    if (!eventFileWriters.ContainsKey(rEvent))
                    {
                        string filePath = fileName + " - " + rEvent + ".csv";
                        eventFileWriters.Add(rEvent, new StreamWriter(filePath));
                        await eventFileWriters[rEvent].WriteLineAsync(headerRecord).ConfigureAwait(false);
                    }
                    string record = await sr.ReadLineAsync().ConfigureAwait(false);
                    await eventFileWriters[rEvent].WriteLineAsync(record).ConfigureAwait(false);
                }
            }

            foreach (StreamWriter sw in eventFileWriters.Values)
            {
                await sw.FlushAsync().ConfigureAwait(false);
                sw.Dispose();
            }
        }
    }
}
