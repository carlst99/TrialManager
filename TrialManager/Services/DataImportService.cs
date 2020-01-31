using CsvHelper;
using Stylet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model.Csv;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Services
{
    public class DataImportService : IDataImportService
    {
        /// <summary>
        /// Imports data from a CSV file, loading it to the database
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="preferredDayMappings">Maps a day string to a defined preferred day object</param>
        /// <exception cref="IOException"></exception>
        public async Task<Tuple<BindableCollection<Trialist>, BindableCollection<DuplicateTrialistEntry>>> ImportFromCsv(string path, Dictionary<string, DateTimeOffset> preferredDayMappings)
        {
            try
            {
                HashSet<int> trialistHashes = new HashSet<int>();
                BindableCollection<Trialist> trialists = new BindableCollection<Trialist>();
                BindableCollection<DuplicateTrialistEntry> duplicates = new BindableCollection<DuplicateTrialistEntry>();

                await foreach (MappedTrialist mt in EnumerateCsv(path))
                {
                    Trialist trialist = mt.ToTrialist(preferredDayMappings);

                    // If we don't have a duplicate
                    if (trialistHashes.Add(trialist.GetHashCode()))
                    {
                        trialists.Add(trialist);
                    }
                    else
                    {
                        Trialist clash = trialists.First(t => t.GetHashCode().Equals(trialist.GetHashCode()));
                        duplicates.Add(new DuplicateTrialistEntry(clash, trialist));
                    }
                }

                return new Tuple<BindableCollection<Trialist>, BindableCollection<DuplicateTrialistEntry>>(trialists, duplicates);
            }
            catch (Exception ex)
            {
                Bootstrapper.LogError("Could not import data from CSV", ex);
                return null;
            }
        }

        /// <summary>
        /// Packages the functionality for mapping and enumerating a CSV file
        /// </summary>
        /// <param name="path">The path to the CSV file</param>
        /// <returns></returns>
        private async IAsyncEnumerable<MappedTrialist> EnumerateCsv(string path)
        {
            using StreamReader reader = new StreamReader(path);
            using CsvReader csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            csv.Configuration.TypeConverterCache.AddConverter<EntityStatus>(new EntityStatusConverter());
            csv.Configuration.RegisterClassMap<TrialistMapping>();

            await foreach (MappedTrialist mt in csv.GetRecordsAsync<MappedTrialist>())
                yield return mt;
        }
    }
}
