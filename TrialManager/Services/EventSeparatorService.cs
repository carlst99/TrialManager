using CsvHelper;
using System;

namespace TrialManager.Services
{
    public class EventSeparatorService
    {
        private readonly ICsvImportService _importService;

        public EventSeparatorService(ICsvImportService importService)
        {
            _importService = importService;
        }

        public void Separate(string path, string header)
        {
            using (CsvReader reader = _importService.GetCsvReader(path))
            {
                throw new NotImplementedException();
            }
        }
    }
}
