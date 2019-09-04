using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TrialManager.Core.Model.LocationDb;

namespace TrialManager.Core.Services
{
    public sealed class LocationService : ILocationService
    {
        /// <summary>
        /// Returns an autocomplete suggestion list
        /// </summary>
        /// <param name="text">The prompt text</param>
        /// <param name="maxCount">The maximum number of suggestions to return</param>
        public List<string> GetAutoCompleteSuggestions(string text, int maxCount = 5)
        {
            Realm realm = GetRealmInstance();

            if (string.IsNullOrEmpty(text))
                return null;
            text = text.ToLower();

            List<string> locations = new List<string>();

            // Get all matching suburbs
            foreach (SuburbLocalityLocation sLoc in realm.All<SuburbLocalityLocation>())
            {
                if (sLoc.FullName.ToLower().StartsWith(text))
                    locations.Add(sLoc.FullName);

                if (locations.Count == maxCount)
                    return locations;
            }

            // If we are here, we haven't yet reached the max count
            // As such, now search through towns/cities
            foreach (TownCityLocation tLoc in realm.All<TownCityLocation>())
            {
                if (tLoc.Name.ToLower().StartsWith(text))
                    locations.Add(tLoc.Name);

                if (locations.Count == maxCount)
                    return locations;
            }

            return locations;
        }

        public bool TryResolve(string text, out ILocation location)
        {
            if (string.IsNullOrEmpty(text))
            {
                location = null;
                return false;
            }

            Realm realm = GetRealmInstance();
            text = text.ToLower();

            // Look first through the smaller towns/cities list
            foreach (TownCityLocation tLoc in realm.All<TownCityLocation>())
            {
                string tLocName = tLoc.Name.ToLower();
                if (text.Contains(tLocName) || text.Equals(tLocName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (SuburbLocalityLocation sLoc in tLoc.Suburbs)
                    {
                        if (text.Contains(sLoc.Name.ToLower()))
                        {
                            location = sLoc;
                            return true;
                        }
                    }

                    location = tLoc;
                    return true;
                } 
            }

            // If no town/city could be found, check through suburbs
            foreach (SuburbLocalityLocation sLoc in realm.All<SuburbLocalityLocation>())
            {
                string sLocName = sLoc.Name.ToLower();
                if (text.Contains(sLocName) || text.Equals(sLocName, StringComparison.OrdinalIgnoreCase))
                {
                    location = sLoc;
                    return true;
                }
            }

            // If we are here the search failed to find any towns/cities
            location = null;
            return false;
        }

        private Realm GetRealmInstance()
        {
            string assemPath = Assembly.GetEntryAssembly().Location;
            assemPath = Path.GetDirectoryName(assemPath);
            string realmPath = Path.Combine(assemPath, "Resources", "locations.realm");

            RealmConfiguration config = new RealmConfiguration(realmPath)
            {
                ObjectClasses = new[] { typeof(SuburbLocalityLocation), typeof(TownCityLocation) }
            };
            return Realm.GetInstance(config);
        }
    }
}
