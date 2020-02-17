using CsvHelper;
using CsvHelper.Configuration;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model;
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
        public async Task<Tuple<BindableCollection<Trialist>, BindableCollection<DuplicateTrialistEntry>>> ImportFromCsv(string path, IList<PreferredDayDateTimePair> preferredDayMappings, TrialistCsvClassMap classMap)
        {
            try
            {
                HashSet<int> trialistHashes = new HashSet<int>();
                BindableCollection<Trialist> trialists = new BindableCollection<Trialist>();
                BindableCollection<DuplicateTrialistEntry> duplicates = new BindableCollection<DuplicateTrialistEntry>();

                await foreach (MappedTrialist mt in EnumerateCsv(path, classMap))
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
        /// Performs a basic check for a CSV file by looking for the separator chars (; or ,)
        /// </summary>
        /// <returns>True if the file appears to be of a valid CSV structure</returns>
        public static async Task<bool> IsValidCsvFile(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            using StreamReader sr = new StreamReader(filePath);
            string line = await sr.ReadLineAsync().ConfigureAwait(false);
            return line.Contains(';') || line.Contains(',');
        }

        /// <summary>
        /// Gets a CSV reader setup with an <see cref="EntityStatusConverter"/> and provided classmap
        /// </summary>
        /// <param name="filePath">The path to the file upon which a CSV reader should be open</param>
        /// <param name="classMap">The classmap to use</param>
        /// <returns></returns>
        public static CsvReader GetCsvReader(string filePath, ClassMap<MappedTrialist> classMap = null)
        {
            StreamReader reader = new StreamReader(filePath);
            CsvReader csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            csv.Configuration.TypeConverterCache.AddConverter<EntityStatus>(new EntityStatusConverter());
            if (classMap != null)
                csv.Configuration.RegisterClassMap(classMap);
            return csv;
        }

        /// <summary>
        /// Creates a class map based on user mappings input
        /// </summary>
        /// <returns></returns>
        public static TrialistCsvClassMap BuildClassMap(IList<PropertyHeaderPair> mappedProperties)
        {
            TrialistCsvClassMap classMap = new TrialistCsvClassMap();
            System.Reflection.PropertyInfo[] properties = classMap.GetType().GetProperties();
            foreach (PropertyHeaderPair pair in mappedProperties)
            {
                System.Reflection.PropertyInfo property = properties.First(p => p.Name == pair.MappedProperty.ToString());
                property.SetValue(classMap, pair.DataFileProperty);
            }
            classMap.SetupMappings();
            return classMap;
        }

        /// <summary>
        /// Gets the header record, if present, from a CSV file
        /// </summary>
        /// <param name="filePath">The path to the CSV file</param>
        /// <returns></returns>
        public static ReadOnlyCollection<string> GetCsvHeaders(string filePath)
        {
            using CsvReader csvReader = GetCsvReader(filePath);
            csvReader.Read();
            csvReader.ReadHeader();
            string[] headerRecord = csvReader.Context.HeaderRecord;
            return new ReadOnlyCollection<string>(headerRecord);
        }

        /// <summary>
        /// Packages the functionality for mapping and enumerating a CSV file
        /// </summary>
        /// <param name="filePath">The path to the CSV file</param>
        /// <returns></returns>
        private async IAsyncEnumerable<MappedTrialist> EnumerateCsv(string filePath, TrialistCsvClassMap classMap)
        {
            using CsvReader csv = GetCsvReader(filePath, classMap);
            await foreach (MappedTrialist mt in csv.GetRecordsAsync<MappedTrialist>())
                yield return mt;
        }
    }
}
