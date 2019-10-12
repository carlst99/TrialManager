using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using TrialManager.Core.Model;
using TrialManager.Core.Model.LocationDb;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Services
{
    public class DrawCreationService : IDrawCreationService
    {
        #region Constants

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

        /// <summary>
        /// Gets the marker used to define a non-preference of day
        /// </summary>
        public static readonly DateTimeOffset NO_PREFERRED_DAY_MARKER = DateTimeOffset.MinValue;

        #endregion

        private readonly ILocationService _locationService;

        public DrawCreationService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public IEnumerable<TrialistDrawEntry> CreateDraw(int maxRunsPerDay, DateTime startDay, string address)
        {
            Realm realm = RealmHelpers.GetRealmInstance();
            _locationService.TryResolve(address, out ILocation trialLocation);
            Dictionary<DateTimeOffset, IEnumerable<Trialist>> trialistDayPairs = new Dictionary<DateTimeOffset, IEnumerable<Trialist>>();

            // Get trialists and order by number of dogs, so those with more dogs are run earlier in the day
            IEnumerable<Trialist> trialists = realm.All<Trialist>();
            trialists = trialists.OrderByDescending(t => t.Dogs.Count);

            // Get all distinct days
            List<DateTimeOffset> distinctDays = trialists.Distinct(new PreferredDayEqualityComparer())
                                                                .OrderBy(t => t.PreferredDay)
                                                                .Select(t => t.PreferredDay)
                                                                .ToList();
            // Setup list of trialists for each day
            foreach (DateTimeOffset day in distinctDays)
            {
                IEnumerable<Trialist> trialistsForSaidDay = trialists.Where(t => t.PreferredDay.Equals(day));
                trialistDayPairs.Add(day, trialistsForSaidDay);
            }

            // Sort each list by location to trial grounds
            if (trialLocation != null)
            {
                List<IEnumerable<Trialist>> temp = new List<IEnumerable<Trialist>>();
                foreach (DateTimeOffset key in trialistDayPairs.Keys)
                {
                    var locationSorted = trialistDayPairs[key]
                        .OrderBy(t => Gd2000Coordinate.DistanceTo(t.Location, trialLocation.Location));
                    temp.Add(locationSorted);
                }

                for (int i = 0; i < distinctDays.Count; i++)
                    trialistDayPairs[distinctDays[i]] = temp[i];
            }

            // Fill days to max run count with those who don't have a preferred day, and build the final list
            IEnumerable<Trialist> finalList = null;
            int noPrefPositionCounter = 0;
            List<Trialist> noPreferredDay = trialistDayPairs[NO_PREFERRED_DAY_MARKER].ToList();
            trialistDayPairs.Remove(NO_PREFERRED_DAY_MARKER);

            foreach (DateTimeOffset key in trialistDayPairs.Keys)
            {
                IEnumerable<Trialist> list = trialistDayPairs[key];
                int runCount = list.Sum(t => t.Dogs.Count);
                if (runCount < maxRunsPerDay)
                {
                    int runDiff = maxRunsPerDay - runCount;
                    int originalPos = noPrefPositionCounter;
                    while (runDiff >= 0)
                    {
                        runDiff -= noPreferredDay[noPrefPositionCounter].Dogs.Count;
                        noPrefPositionCounter++;
                    }

                    list = list.Concat(noPreferredDay.GetRange(originalPos, noPrefPositionCounter - originalPos));
                }

                if (finalList == null)
                    finalList = list;
                else
                    finalList = finalList.Concat(list);
            }

            if (noPrefPositionCounter < noPreferredDay.Count)
                finalList = finalList.Concat(noPreferredDay.GetRange(noPrefPositionCounter, noPreferredDay.Count - noPrefPositionCounter));

            foreach (TrialistDrawEntry value in SortGeneric(finalList, realm, maxRunsPerDay, 0))
                yield return value;

            yield break;
        }

        private IEnumerable<Trialist> SortByDistance(IEnumerable<Trialist> list, Gd2000Coordinate trialLocation)
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

                if (Gd2000Coordinate.DistanceTo(trialLocation, element.Location) < LOCAL_DISTANCE_MAX)
                    locals.Add(element);
                else
                    nonLocals.Add(element);
            }

            locals.AddRange(unsorted);
            locals.AddRange(nonLocals);
            return locals;
        }

        private IEnumerable<TrialistDrawEntry> SortGeneric(IEnumerable<Trialist> trialists, Realm realm, int maxRunsPerDay, int count)
        {
            TrialistDrawEntry[] draw = new TrialistDrawEntry[realm.All<Dog>().Count() * 2];
            HashSet<int> usedNumbers = new HashSet<int>();
            int startCount = count;

            foreach (Trialist element in trialists)
            {
                // Search for the next available number
                while (!usedNumbers.Add(count))
                    count++;

                // Set the local count and day
                int localCount = count++;
                int dayIncrements = 0;

                // Add each dog with a run spacing of DOG_RUN_SEPARATION
                for (int i = 0; i < element.Dogs.Count; i++)
                {
                    // If we are a multiple of the maximum dogs per day, move to the next day
                    if (i != 0 && i % MAX_DOGS_PER_DAY == 0 && i / MAX_DOGS_PER_DAY == dayIncrements)
                    {
                        // Increment to next day
                        int increment = maxRunsPerDay - localCount;
                        localCount += increment;
                        dayIncrements++;

                        // Find next available position
                        FindNextAvailable(usedNumbers, ref localCount);
                    }

                    // Add the entry
                    draw[localCount] = new TrialistDrawEntry(element, element.Dogs[i], localCount + 1);
                    usedNumbers.Add(localCount);

                    // Increment day if necessary and find next local count
                    localCount += DOG_RUN_SEPARATION;
                    if (localCount != 0 && maxRunsPerDay / localCount < 1)
                        dayIncrements++;
                    FindNextAvailable(usedNumbers, ref localCount);
                }
            }

            int nullCount = 0;
            for (int i = 0; i < draw.Length; i++)
            {
                TrialistDrawEntry element = draw[i];

                if (element == null)
                {
                    nullCount++;
                }
                else
                {
                    element.RunNumber -= nullCount;
                    element.RunNumber += startCount;
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Finds the next available draw position
        /// </summary>
        /// <param name="usedNumbers"></param>
        /// <param name="localCount"></param>
        private void FindNextAvailable(HashSet<int> usedNumbers, ref int localCount)
        {
            while (!usedNumbers.Add(localCount))
                localCount++;
            usedNumbers.Remove(localCount);
        }
    }
}
