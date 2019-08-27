﻿using CsvHelper;
using MvvmCross.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Core.Model.Csv;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly TrialistContext _trialistContext;
        private readonly IMvxMainThreadAsyncDispatcher _asyncDispatcher;

        public DataImportService(ITrialistContext tContext, IMvxMainThreadAsyncDispatcher asyncDispatcher)
        {
            _trialistContext = (TrialistContext)tContext;
            _asyncDispatcher = asyncDispatcher;

        }

        /// <summary>
        /// Imports data from a CSV file, loading it to the database
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="merge">Whether the import should be merged with existing data</param>
        /// <exception cref="IOException"></exception>
        public async Task<bool> ImportFromCsv(string path, bool merge)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!merge)
                        ClearExistingData().ConfigureAwait(false);

                    HashSet<int> trialistHashes = new HashSet<int>();
                    List<MappedTrialist> duplicates = new List<MappedTrialist>();

                    // First pass, to get trialists
                    foreach (MappedTrialist mt in EnumerateCsv(path))
                    {
                        if (trialistHashes.Add(mt.GetHashCode()))
                            EOMTA(() => _trialistContext.Trialists.Add(mt.ToTrialist())).ConfigureAwait(false);
                        else
                            duplicates.Add(mt);
                    }

                    _trialistContext.SaveChangesAsync().ConfigureAwait(false);

                    if (duplicates.Count <= 0)
                    {
                        SetupTravellingPartnersFromCsv(path).ConfigureAwait(false);
                    }
                    else
                    {
                        // TODO ask user to manage duplicates
                    }

                    return true;
                } catch (Exception ex)
                {
                    App.LogError("Could not import data from CSV", ex);
                    return false;
                }
            });
        }

        /// <summary>
        /// Clears existing data in the database
        /// </summary>
        /// <returns></returns>
        public async Task ClearExistingData()
        {
            int count = _trialistContext.Trialists.Count();
            for (int i = 0; i < count; i++)
            {
                await EOMTA(() => _trialistContext.Trialists.RemoveRange(_trialistContext.Trialists.ToList())).ConfigureAwait(false);
                await _trialistContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Performs a second pass over the data to setup travelling partners. This method should only be called after <see cref="ImportFromCsv(string, bool)"/>
        /// </summary>
        /// <param name="path">The path to the CSV file</param>
        /// <returns></returns>
        private async Task SetupTravellingPartnersFromCsv(string path)
        {
            await Task.Run(() =>
            {
                // Second pass, to setup travelling partner
                foreach (MappedTrialist mt in EnumerateCsv(path))
                {
                    if (string.IsNullOrEmpty(mt.TravellingPartner))
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
                    EOMTA(() => _trialistContext.Trialists.Update(toUpdate)).ConfigureAwait(false);
                }
                _trialistContext.SaveChangesAsync().ConfigureAwait(false);
            });
        }

        private async Task EOMTA(Action action) => await _asyncDispatcher.ExecuteOnMainThreadAsync(action).ConfigureAwait(false);

        /// <summary>
        /// Packages the functionality for mapping and enumerating a CSV file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerable<MappedTrialist> EnumerateCsv(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader))
            {
                csv.Configuration.TypeConverterCache.AddConverter<EntityStatus>(new EntityStatusConverter());
                csv.Configuration.RegisterClassMap<TrialistMapping>();

                // Second pass, to setup travelling partner
                foreach (MappedTrialist mt in csv.GetRecords<MappedTrialist>())
                    yield return mt;

            }
        }
    }
}
