using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TrialManager.Core.Model;
using TrialManager.Core.Model.LocationDb;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Services
{
    public class DrawCreationService : IDrawCreationService
    {
        /// <summary>
        /// Defines the maximum Gd2000 that a trialist can be considered as local for
        /// </summary>
        public const double LOCAL_DISTANCE_MAX = 0.6;

        /// <summary>
        /// Defines the maximum number of dogs that may be run in a day
        /// </summary>
        public const int MAX_DOGS_PER_DAY = 3;

        /// <summary>
        /// Defines the separation between dogs in the draw
        /// </summary>
        public const int DOG_RUN_SEPARATION = 20;

        private readonly ILocationService _locationService;

        public DrawCreationService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public IEnumerable<TrialistDrawEntry> CreateDraw(int maxRunsPerDay, DateTime startDay, string address)
        {
            Realm realm = RealmHelpers.GetRealmInstance();
            _locationService.TryResolve(address, out ILocation location);
            //SortedDictionary<DateTimeOffset, List<Trialist>> days = new SortedDictionary<DateTimeOffset, List<Trialist>>();
            //List<Trialist> noPreferredDay = new List<Trialist>();

            // Sort all trialists according to preferred day
            //foreach (Trialist element in realm.All<Trialist>())
            //{
            //    if (element.PreferredDay != null && !days.ContainsKey(element.PreferredDay))
            //        days.Add(element.PreferredDay, new List<Trialist>());

            //    if (element.PreferredDay != null)
            //    {
            //        if (days[element.PreferredDay].Count() < maxRunsPerDay)
            //            days[element.PreferredDay].Add(element);
            //        else
            //            noPreferredDay.Add(element);
            //    } else
            //    {
            //        noPreferredDay.Add(element);
            //    }
            //}

            //// Sort trialists who did not specify a preferred day
            //foreach (Trialist element in noPreferredDay)
            //{
            //    bool wasPlaced = false;

            //    foreach (var pair in days)
            //    {
            //        if (pair.Value.Count() < maxRunsPerDay)
            //        {
            //            pair.Value.Add(element);
            //            wasPlaced = true;
            //            break;
            //        }
            //    }

            //    // If all other days were full we need to create a new day to place the trialist
            //    if (!wasPlaced)
            //    {
            //        DateTimeOffset key = days.Keys.Max().AddDays(1);
            //        days.Add(key, new List<Trialist>());
            //        days[key].Add(element);
            //    }
            //}

            // Sort within days by distance to trial grounds
            //foreach (DateTimeOffset key in days.Keys)
            //    days[key] = SortByDistance(days[key], location.Location);

            //foreach (var pair in days)
            //{
            //    foreach (Trialist element in pair.Value)
            //    {
            //        foreach (Dog dog in element.Dogs)
            //            yield return new TrialistDrawEntry(element, dog, count++, pair.Key);
            //    }
            //}

            IEnumerable<Trialist> trialists;
            if (location != null)
                trialists = SortByDistance(realm.All<Trialist>(), location.Location);
            else
                trialists = realm.All<Trialist>();

            foreach (TrialistDrawEntry value in SortGeneric(trialists, realm, maxRunsPerDay, startDay))
                yield return value;
        }

        private IEnumerable<Trialist> SortByDistance(IEnumerable<Trialist> list, Location trialLocation)
        {
            List<Trialist> locals = new List<Trialist>();
            List<Trialist> nonLocals = new List<Trialist>();
            List<Trialist> unsorted = new List<Trialist>();

            foreach (Trialist element in list)
            {
                if (element.Location == null)
                {
                    unsorted.Add(element);
                    continue;
                }

                if (Location.DistanceTo(trialLocation, element.Location) < LOCAL_DISTANCE_MAX)
                    locals.Add(element);
                else
                    nonLocals.Add(element);
            }

            locals.AddRange(unsorted);
            locals.AddRange(nonLocals);
            return locals;
        }

        private IEnumerable<TrialistDrawEntry> SortGeneric(IEnumerable<Trialist> trialists, Realm realm, int maxRunsPerDay, DateTimeOffset startDay)
        {
            TrialistDrawEntry[] draw = new TrialistDrawEntry[realm.All<Dog>().Count() * 2];
            HashSet<int> usedNumbers = new HashSet<int>();
            int count = 0;
            DateTimeOffset day = startDay;
            int oCount = 0;

            foreach (Trialist element in trialists)
            {
                // Search for the next available number and update day if required
                while (!usedNumbers.Add(count))
                {
                    count++;
                    if (count % maxRunsPerDay == 0)
                        day = day.AddDays(1);
                }

                // Set the local count and day
                int localCount = count++;
                if (count % maxRunsPerDay == 0)
                    day = day.AddDays(1);
                DateTimeOffset localDay = day;

                // Add each dog with a run spacing of DOG_RUN_SEPARATION
                for (int i = 0; i < element.Dogs.Count; i++)
                {
                    // If we are a multiple of the maximum dogs per day, move to the next day
                    if (i != 0 && i % MAX_DOGS_PER_DAY == 0)
                    {
                        // Increment to next day
                        int increment = maxRunsPerDay - localCount;
                        localCount += increment;
                        localDay = localDay.AddDays(1);

                        // Find next available position
                        while (!usedNumbers.Add(localCount))
                            localCount++;
                        usedNumbers.Remove(localCount); // Remove used number as it will be added later
                    }

                    // Add the entry
                    if (draw[localCount] != null)
                    {
                        oCount++;
                        Debug.WriteLine(localCount);
                    }

                    draw[localCount] = new TrialistDrawEntry(element, element.Dogs[i], localCount + 1, localDay);
                    usedNumbers.Add(localCount);
                    localCount += DOG_RUN_SEPARATION;
                }
            }

            Debug.WriteLine(oCount);

            int nullCount = 0;
            foreach (TrialistDrawEntry element in draw)
            {
                if (element == null)
                {
                    nullCount++;
                }
                else
                {
                    element.RunNumber -= nullCount;
                    yield return element;
                }
            }
        }
    }
}
