using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model;
using TrialManager.Model.Csv;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly ILocationService _locationService;
        private IEnumerable<MappedTrialist> _tempStorageList;

        public CsvImportService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Enumerates the distinct preferred day strings in a trialist CSV file
        /// </summary>
        /// <param name="path">The path to the CSV file</param>
        /// <param name="classMap">The classmap used to create a <see cref="MappedTrialist"/> object</param>
        public async Task<List<string>> GetDistinctPreferredDays(string path, ClassMap<MappedTrialist> classMap)
        {
            return await Task.Run(() =>
            {
                HashSet<int> dayHashes = new HashSet<int>();
                List<string> days = new List<string>();
                foreach (MappedTrialist mt in EnumerateCsv(path, classMap))
                {
                    if (dayHashes.Add(mt.PreferredDayString.GetHashCode()))
                        days.Add(mt.PreferredDayString);
                }
                return days;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Imports data from a CSV file as a collection of <see cref="MappedTrialist"/> and builds a list of duplicates to resolve
        /// </summary>
        /// <param name="path">The path to the csv file</param>
        /// <param name="classMap">The classmap used to create a <see cref="MappedTrialist"/> object</param>
        public async Task<List<DuplicateTrialistPair>> GetMappedDuplicates(string path, ClassMap<MappedTrialist> classMap)
        {
            return await Task.Run(() =>
            {
                HashSet<int> trialistHashes = new HashSet<int>();
                List<MappedTrialist> tempStorageList = new List<MappedTrialist>();
                List<DuplicateTrialistPair> pairs = new List<DuplicateTrialistPair>();

                // Note that this algorithm can result in multiple duplicate fields if the trialist has submitted more than two entries
                // However as this is unlikely, the user should have enough short-term memory to realise that there are 'duplicate duplicates'
                foreach (MappedTrialist mt in EnumerateCsv(path, classMap))
                {
                    // If we can add the hash, then we have not yet discovered a potential duplicate
                    if (trialistHashes.Add(mt.GetHashCode()))
                    {
                        tempStorageList.Add(mt);
                    }
                    else
                    {
                        MappedTrialist clash = tempStorageList.First(t => t.GetHashCode().Equals(mt.GetHashCode()));
                        clash.HasDuplicateClash = true;
                        pairs.Add(new DuplicateTrialistPair(clash, mt));
                    }
                }
                _tempStorageList = tempStorageList.Where(t => !t.HasDuplicateClash);
                return pairs;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Builds a <see cref="Trialist"/> list, factoring in resolved duplicates
        /// </summary>
        /// <param name="duplicates">A list of resolved duplicates</param>
        /// <param name="preferredDayMappings">Preferred day mappings</param>
        public async Task<List<Trialist>> BuildTrialistList(IList<DuplicateTrialistPair> duplicates, IList<PreferredDayDateTimePair> preferredDayMappings)
        {
            return await Task.Run(() =>
            {
                if (preferredDayMappings is null)
                {
                    preferredDayMappings = new List<PreferredDayDateTimePair>
                {
                    new PreferredDayDateTimePair(null, DateTime.MinValue)
                };
                }
                List<Trialist> trialists = new List<Trialist>();
                foreach (MappedTrialist trialist in _tempStorageList)
                    trialists.Add(trialist.ToTrialist(_locationService, preferredDayMappings));

                foreach (DuplicateTrialistPair pair in duplicates)
                {
                    if (pair.KeepFirstTrialist)
                        trialists.Add(pair.FirstTrialist.ToTrialist(_locationService, preferredDayMappings));
                    if (pair.KeepSecondTrialist)
                        trialists.Add(pair.SecondTrialist.ToTrialist(_locationService, preferredDayMappings));
                }
                return trialists;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Performs a basic check for a CSV file by looking for the separator chars (; or ,)
        /// </summary>
        /// <returns>True if the file appears to be of a valid CSV structure</returns>
        public async Task<bool> IsValidCsvFile(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line = await sr.ReadLineAsync().ConfigureAwait(false);
                return line?.Contains(';') == true || line?.Contains(',') == true;
            }
        }

        /// <summary>
        /// Gets a CSV reader setup with an <see cref="EntityStatusConverter"/> and provided classmap
        /// </summary>
        /// <param name="filePath">The path to the file upon which a CSV reader should be open</param>
        /// <param name="classMap">The classmap to use</param>
        /// <returns></returns>
        public CsvReader GetCsvReader(string filePath, ClassMap<MappedTrialist> classMap = null)
        {
            StreamReader reader = new StreamReader(filePath);
            CsvReader csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            csv.Configuration.TypeConverterCache.AddConverter<string>(new StringTrimConverter());
            if (classMap != null)
                csv.Configuration.RegisterClassMap(classMap);
            return csv;
        }

        /// <summary>
        /// Creates a class map based on user mappings input
        /// </summary>
        /// <returns></returns>
        public TrialistCsvClassMap BuildClassMap(IList<PropertyHeaderPair> mappedProperties, IList<PropertyHeaderPair> optionalProperties, string defaultValue)
        {
            TrialistCsvClassMap classMap = new TrialistCsvClassMap();
            System.Reflection.PropertyInfo[] properties = classMap.GetType().GetProperties();
            foreach (PropertyHeaderPair pair in mappedProperties)
            {
                System.Reflection.PropertyInfo property = properties.First(p => p.Name == pair.MappedProperty.ToString());
                property.SetValue(classMap, pair.DataFileProperty);
            }
            classMap.SetupMappings();

            if (optionalProperties != null)
            {
                foreach (PropertyHeaderPair pair in optionalProperties.Where(p => p.DataFileProperty != defaultValue))
                {
                    System.Reflection.PropertyInfo property = properties.First(p => p.Name == pair.OptionalMappedProperty.ToString());
                    property.SetValue(classMap, pair.DataFileProperty);
                }
                classMap.SetupOptionalMappings();
            }
            return classMap;
        }

        /// <summary>
        /// Gets the header record, if present, from a CSV file
        /// </summary>
        /// <param name="filePath">The path to the CSV file</param>
        /// <returns></returns>
        public string[] GetCsvHeaders(string filePath)
        {
            using (CsvReader csvReader = GetCsvReader(filePath))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                string[] headers = new string[csvReader.Context.HeaderRecord.Length];
                csvReader.Context.HeaderRecord.CopyTo(headers, 0);
                return headers;
            }
        }

        /// <summary>
        /// Enumerates a collection of <see cref="MappedTrialist"/> objects from the specified CSV file
        /// </summary>
        /// <param name="filePath">The path to the CSV file</param>
        /// <param name="classMap">The classmap used to create the <see cref="MappedTrialist"/> object</param>
        private IEnumerable<MappedTrialist> EnumerateCsv(string filePath, ClassMap<MappedTrialist> classMap)
        {
            using (CsvReader csv = GetCsvReader(filePath, classMap))
            {
                foreach (MappedTrialist mt in csv.GetRecords<MappedTrialist>())
                    yield return mt;
            }
        }
    }
}
