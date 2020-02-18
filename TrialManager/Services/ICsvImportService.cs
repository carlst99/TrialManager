using CsvHelper;
using CsvHelper.Configuration;
using Stylet;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TrialManager.Model;
using TrialManager.Model.Csv;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Services
{
    public interface ICsvImportService
    {
        /// <summary>
        /// Imports data from a CSV file as a collection of <see cref="MappedTrialist"/> and builds a list of duplicates to resolve
        /// </summary>
        /// <param name="path">The path to the csv file</param>
        Task<BindableCollection<DuplicateTrialistPair>> GetMappedDuplicates(string path, TrialistCsvClassMap classMap);

        /// <summary>
        /// Compresses a <see cref="DuplicateTrialistPair"/> list and adds preferred day data
        /// </summary>
        /// <param name="duplicates"></param>
        /// <param name="preferredDayMappings"></param>
        /// <returns></returns>
        Task<BindableCollection<Trialist>> FinaliseTrialistList(IList<DuplicateTrialistPair> duplicates, IList<PreferredDayDateTimePair> preferredDayMappings);

        /// <summary>
        /// Performs a basic check for a CSV file by looking for the separator chars (; or ,)
        /// </summary>
        /// <returns>True if the file appears to be of a valid CSV structure</returns>
        Task<bool> IsValidCsvFile(string filePath);

        /// <summary>
        /// Gets a CSV reader setup with an <see cref="EntityStatusConverter"/> and provided classmap
        /// </summary>
        /// <param name="filePath">The path to the file upon which a CSV reader should be open</param>
        /// <param name="classMap">The classmap to use</param>
        /// <returns></returns>
        CsvReader GetCsvReader(string filePath, ClassMap<MappedTrialist> classMap = null);

        /// <summary>
        /// Creates a class map based on user mappings input
        /// </summary>
        /// <returns></returns>
        TrialistCsvClassMap BuildClassMap(IList<PropertyHeaderPair> mappedProperties);

        /// <summary>
        /// Gets the header record, if present, from a CSV file
        /// </summary>
        /// <param name="filePath">The path to the CSV file</param>
        /// <returns></returns>
        ReadOnlyCollection<string> GetCsvHeaders(string filePath);
    }
}
