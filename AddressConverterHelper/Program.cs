using AddressConverterHelper.Model;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;

namespace AddressConverterHelper
{
    public static class Program
    {
        public const char CSV_SPLIT_CHAR = ',';
        public const string CSV_FILE_LOCATION = "Resources\\nz-street-address.csv";

        public static void Main()
		{
            LocationContext locations = new LocationContext();
            // Delete previous database
            Console.WriteLine("Deleting previous database...");
            locations.Database.EnsureDeleted();

            // Create new database
            Console.WriteLine("Creating new database...");
            locations.Database.EnsureCreated();

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
                        suburbLocalities[sLoc.Name].Merge(sLoc);
                    else
                        suburbLocalities.Add(sLoc.Name, sLoc);
                }
            }

            // Prepare each suburb/locality for DB storage
            // Also now generate a list of towns/cities
            Console.WriteLine("Preparing suburbs/localities and creating list of towns/cities...");
            Dictionary<string, TownCityLocation> townsCities = new Dictionary<string, TownCityLocation>();
            foreach (SuburbLocalityLocation sLoc in suburbLocalities.Values)
            {
                sLoc.Prepare();
                locations.SuburbsLocalities.Add(sLoc);

                if (townsCities.ContainsKey(sLoc.TownCityName))
                {
                    townsCities[sLoc.TownCityName].Suburbs.Add(sLoc);
                }
                else if (!string.IsNullOrEmpty(sLoc.TownCityName))
                {
                    TownCityLocation tLoc = new TownCityLocation
                    {
                        Name = sLoc.TownCityName
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
                locations.TownsCities.Add(tLoc);
            }

            Console.WriteLine("Saving DB. This may take a while...");
            locations.SaveChanges();
            Console.WriteLine("DB saved. Press any key to exit");
            Console.ReadLine();

            locations.Dispose();
		}
    }
}
