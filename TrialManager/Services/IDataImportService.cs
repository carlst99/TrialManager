using Realms;
using Stylet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrialManager.Model.Csv;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Services
{
    public interface IDataImportService
    {
        /// <summary>
        /// Imports data from a CSV file, loading it to the database
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="merge">Whether the import should be merged with existing data</param>
        /// <exception cref="IOException"></exception>
        Task<Tuple<BindableCollection<Trialist>, BindableCollection<DuplicateTrialistEntry>>> ImportFromCsv(string path, Dictionary<string, DateTimeOffset> preferredDayMappings);
    }
}
