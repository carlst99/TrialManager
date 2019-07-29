using System.Collections.Generic;
using TrialManager.Core.Model.LocationDb;

namespace TrialManager.Core.Services
{
    public sealed class LocationService : ILocationService
    {
        public const char SEPARATOR_CHAR = ',';

        private readonly ILocationContext _locations;

        public LocationService(ILocationContext locations)
        {
            _locations = locations;
        }

        /// <summary>
        /// Returns an autocomplete suggestion list
        /// </summary>
        /// <param name="text">The prompt text</param>
        /// <param name="maxCount">The maximum number of suggestions to return</param>
        public List<string> GetAutoCompleteSuggestions(string text, int maxCount = 5)
        {
            text.ToLower();
            if (string.IsNullOrEmpty(text))
                return null;
            List<string> locations = new List<string>();

            // Get all matching suburbs
            foreach (SuburbLocalityLocation sLoc in _locations.SuburbsLocalities)
            {
                if (sLoc.FullName.ToLower().StartsWith(text))
                    locations.Add(sLoc.FullName);

                if (locations.Count == maxCount)
                    return locations;
            }

            // If we are here, we haven't yet reached the max count
            // As such, now search through towns/cities
            foreach (TownCityLocation tLoc in _locations.TownsCities)
            {
                if (tLoc.Name.ToLower().StartsWith(text))
                    locations.Add(tLoc.Name);

                if (locations.Count == maxCount)
                    return locations;
            }

            return locations;
        }

        public bool TryResolve(string text, out LocationBase location)
        {
            text.ToLower();

            // Look first through the smaller towns/cities list
            foreach (TownCityLocation tLoc in _locations.TownsCities)
            {
                if (text.Contains(tLoc.Name.ToLower()))
                {
                    foreach (SuburbLocalityLocation sLoc in tLoc.Suburbs)
                    {
                        if (text.Contains(sLoc.Name.ToLower()))
                        {
                            location = sLoc;
                            return true;
                        } else
                        {
                            location = tLoc;
                            return true;
                        }
                    }
                }
            }

            // If no town/city could be found, check through suburbs
            foreach (SuburbLocalityLocation sLoc in _locations.SuburbsLocalities)
            {
                if (text.Contains(sLoc.Name.ToLower()))
                {
                    location = sLoc;
                    return true;
                }
            }

            // If we are here the search failed to find any towns/cities
            location = null;
            return false;

            //// Check first for a combined address
            //if (text.Contains(SEPARATOR_CHAR.ToString()))
            //{
            //    string[] components = text.Split(SEPARATOR_CHAR);
            //    TownCityLocation tLoc = MatchTownCity(components[0].Trim());
            //} else
            //{
            //    TownCityLocation tLoc = MatchTownCity(text.Trim());
            //    if (tLoc != null)
            //    {
            //        location = tLoc;
            //        return true;
            //    }

            //    SuburbLocalityLocation sLoc = MatchSuburbLocality(text.Trim());
            //    if (sLoc != null)
            //    {
            //        location = sLoc;
            //        return true;
            //    }

            //    location = null;
            //    return false;
            //}
        }

        private TownCityLocation MatchTownCity(string text)
        {
            foreach (TownCityLocation tLoc in _locations.TownsCities)
            {
                if (tLoc.Name.ToLower() == text)
                    return tLoc;
            }

            return null;
        }
    }
}
