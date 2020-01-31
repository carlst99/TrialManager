﻿using Realms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<bool> ImportFromCsv(
            string path,
            bool merge,
            Dictionary<string, DateTimeOffset> preferredDayMappings);

        /// <summary>
        /// Clears existing data in the database
        /// </summary>
        /// <returns></returns>
        Task ClearExistingData(Realm realm = null);
    }
}