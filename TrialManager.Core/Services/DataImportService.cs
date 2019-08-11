using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrialManager.Core.Model.Csv;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly TrialistContext _trialistContext;

        public DataImportService(ITrialistContext tContext)
        {
            _trialistContext = (TrialistContext)tContext;
        }

        public async void ImportFromCsv(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader))
            {
                csv.Configuration.TypeConverterCache.AddConverter<EntityStatus>(new EntityStatusConverter());
                csv.Configuration.RegisterClassMap<TrialistMapping>();

                // First pass, to get trialists
                foreach (MappedTrialist mt in csv.GetRecords<MappedTrialist>())
                    _trialistContext.Trialists.Add(mt.ToTrialist());
                await _trialistContext.SaveChangesAsync().ConfigureAwait(false);

                // Second pass, to setup travelling partner
                foreach (MappedTrialist mt in csv.GetRecords<MappedTrialist>())
                {
                    if (string.IsNullOrEmpty(mt.Address))
                        continue;

                    // Find one potential partner
                    IEnumerable<Trialist> partners = _trialistContext.Trialists.Where(t => t.FullName.Equals(mt.TravellingPartner, StringComparison.OrdinalIgnoreCase));
                    if (partners.Count() != 1)
                        continue;

                    // Find one original
                    Trialist trialist = mt.ToTrialist();
                    IEnumerable<Trialist> trialists = _trialistContext.Trialists.Where(t => t.IsContentEqual(trialist));
                    if (trialists.Count() != 1)
                        return;

                    Trialist toUpdate = trialists.First();
                    toUpdate.TravellingPartner = partners.First();
                    _trialistContext.Trialists.Update(toUpdate);
                }
                await _trialistContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
