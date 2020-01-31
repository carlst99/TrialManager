﻿using CsvHelper;
using Realms;
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
    public class DataImportService : IDataImportService
    {
        /// <summary>
        /// Imports data from a CSV file, loading it to the database
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="merge">Whether the import should be merged with existing data</param>
        /// <exception cref="IOException"></exception>
        public async Task<bool> ImportFromCsv(
            string path,
            bool merge,
            Dictionary<string, DateTimeOffset> preferredDayMappings)
        {
            try
            {
                Realm realm = RealmHelpers.GetRealmInstance();

                HashSet<int> trialistHashes = new HashSet<int>();
                List<Trialist> duplicates = new List<Trialist>();

                if (!merge)
                {
                    await ClearExistingData(realm).ConfigureAwait(true);
                } else
                {
                    foreach (Trialist t in realm.All<Trialist>())
                        trialistHashes.Add(t.GetContentHashCode());
                }

                // First pass, to get trialists
                await realm.WriteAsync(tempRealm =>
                {
                    foreach (MappedTrialist mt in EnumerateCsv(path))
                    {
                        Trialist trialist = mt.ToTrialist(tempRealm, preferredDayMappings);

                        if (trialistHashes.Add(trialist.GetContentHashCode()))
                            tempRealm.Add(trialist);
                        else
                            tempRealm.Add(trialist);
                            duplicates.Add(trialist);
                    }
                }).ConfigureAwait(true);

                if (duplicates.Count == 0)
                {
                    return SetupTravellingPartnersFromCsv(path).Result;
                }
                else
                {
                    // TODO ask user to manage duplicates
                }

                return true;
            }
            catch (Exception ex)
            {
                Bootstrapper.LogError("Could not import data from CSV", ex);
                return false;
            }
        }

        /// <summary>
        /// Clears existing data in the database
        /// </summary>
        /// <returns></returns>
        public async Task ClearExistingData(Realm realm = null)
        {
            if (realm == null)
                realm = RealmHelpers.GetRealmInstance();

            RealmHelpers.ClearNextIds();
            await realm.WriteAsync(tempRealm => tempRealm.RemoveAll<Trialist>()).ConfigureAwait(true);
        }

        /// <summary>
        /// Performs a second pass over the data to setup travelling partners. This method should only be called after <see cref="ImportFromCsv(string, bool)"/>
        /// </summary>
        /// <param name="path">The path to the CSV file</param>
        /// <returns></returns>
        private async Task<bool> SetupTravellingPartnersFromCsv(string path)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Realm realm = RealmHelpers.GetRealmInstance();
                    realm.Write(() =>
                    {
                        // Second pass, to setup travelling partner
                        foreach (MappedTrialist mt in EnumerateCsv(path))
                        {
                            if (string.IsNullOrEmpty(mt.TravellingPartner))
                                continue;

                            // Find one potential partner
                            IQueryable<Trialist> partners = realm.All<Trialist>().Where(t => t.Name.Equals(mt.TravellingPartner, StringComparison.OrdinalIgnoreCase));
                            if (partners.Count() != 1)
                                continue;

                            // Find one original
                            IQueryable<Trialist> trialists = realm.All<Trialist>().Where(t => t.Name.Equals(mt.FullName, StringComparison.OrdinalIgnoreCase));
                            if (trialists.Count() != 1)
                                continue;

                            Trialist toUpdate = trialists.First();
                            Trialist partner = partners.First();

                            //  Don't allow circular dependencies
                            if (toUpdate.Equals(partner))
                                continue;

                            toUpdate.TravellingPartner = partner;
                        }
                    });
                    return true;
                }
                catch (Exception ex)
                {
                    Bootstrapper.LogError("Import error - could not link travelling partners", ex);
                    return false;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Packages the functionality for mapping and enumerating a CSV file
        /// </summary>
        /// <param name="path">The path to the CSV file</param>
        /// <returns></returns>
        private IEnumerable<MappedTrialist> EnumerateCsv(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.CurrentCulture))
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