using LINZCsvConverter.Model;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using Realms;
using System.Reflection;

namespace LINZCsvConverter
{
    public static class Program
    {
        public const char CSV_SPLIT_CHAR = ',';
        public const string CSV_FILE_LOCATION = "Resources\\nz-street-address.csv";

        public static void Main()
		{
            Realm realm = null;
            string assemPath = Assembly.GetEntryAssembly().Location;
            assemPath = Path.GetDirectoryName(assemPath);
            string realmPath = Path.Combine(assemPath, "Resources", "locations.realm");

            // Delete previous database
            Console.WriteLine("Deleting previous database...");
            if (File.Exists(realmPath))
                File.Delete(realmPath);

            // Create new database
            Console.WriteLine("Creating new database...");
            realm = Realm.GetInstance(new RealmConfiguration(realmPath));

            int sLocCounter = 0;
            int tLocCounter = 0;

            // Get all of the suburb/localities
            Console.WriteLine("Reading all suburbs/localities...");
            Dictionary<string, SuburbLocalityLocation> suburbLocalities = new Dictionary<string, SuburbLocalityLocation>();
            using (StreamReader reader = new StreamReader(CSV_FILE_LOCATION))
            using (CsvReader cReader = new CsvReader(reader))
            {
                cReader.Configuration.HeaderValidated = null;
                cReader.Configuration.MissingFieldFound = null;

                foreach (SuburbLocalityLocation sLoc in cReader.GetRecords<SuburbLocalityLocation>())
                {
                    // Merge if already present in dictionary
                    if (suburbLocalities.ContainsKey(sLoc.Name))
                    {
                        suburbLocalities[sLoc.Name].Merge(sLoc);
                    }
                    else
                    {
                        sLoc.Id = sLocCounter++;
                        suburbLocalities.Add(sLoc.Name, sLoc);
                    }
                }
            }

            // Prepare each suburb/locality for DB storage
            // Also now generate a list of towns/cities
            Console.WriteLine("Preparing suburbs/localities and creating list of towns/cities...");
            Dictionary<string, TownCityLocation> townsCities = new Dictionary<string, TownCityLocation>();
            realm.Write(() =>
            {
                foreach (SuburbLocalityLocation sLoc in suburbLocalities.Values)
                {
                    sLoc.Prepare();
                    realm.Add(sLoc);

                    if (townsCities.ContainsKey(sLoc.TownCityName))
                    {
                        townsCities[sLoc.TownCityName].Suburbs.Add(sLoc);
                    }
                    else if (!string.IsNullOrEmpty(sLoc.TownCityName))
                    {
                        TownCityLocation tLoc = new TownCityLocation
                        {
                            Name = sLoc.TownCityName,
                            Id = tLocCounter++
                        };
                        tLoc.Suburbs.Add(sLoc);
                        townsCities.Add(tLoc.Name, tLoc);
                    }
                }

                // Prepare each town/city
                Console.WriteLine("Preparing towns/cities...");
                foreach (TownCityLocation tLoc in townsCities.Values)
                {
                    tLoc.Prepare();
                    realm.Add(tLoc);
                }
            });

            Console.WriteLine("DB saved. Press any key to exit");
            Console.ReadLine();
		}
    }
}
