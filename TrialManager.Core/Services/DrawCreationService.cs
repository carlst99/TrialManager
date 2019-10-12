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

            // Get trialists and order by number of dogs, so those with more dogs are run earlier in the day
            IEnumerable<Trialist> trialists = realm.All<Trialist>();
            trialists = trialists.OrderByDescending(t => t.Dogs.Count);

            // Split the trialist list by day
            List<DayTrialistPair> dayTrialistPairs = GetDayTrialistPairs(trialists);

            // Sort each list by distance to trial grounds
            if (trialLocation != null)
                SortByDistance(dayTrialistPairs, trialLocation.Location);

            // Fill days if needed and generate final list
            IEnumerable<Trialist> finalList = GenerateFinalList(dayTrialistPairs, maxRunsPerDay);

            foreach (TrialistDrawEntry value in SpreadAndGenerateRuns(finalList, realm, maxRunsPerDay, 0))
                yield return value;
        }

        /// <summary>
        /// Splits the trialist list by day
        /// </summary>
        /// <param name="trialists"></param>
        /// <returns></returns>
        private List<DayTrialistPair> GetDayTrialistPairs(IEnumerable<Trialist> trialists)
        {
            List<DayTrialistPair> dayTrialistPairs = new List<DayTrialistPair>();
            // Get all distinct days
            List<DateTimeOffset> distinctDays = trialists.Distinct(new PreferredDayEqualityComparer())
                                                                .OrderBy(t => t.PreferredDay)
                                                                .Select(t => t.PreferredDay)
                                                                .ToList();
            // Setup list of trialists for each day
            foreach (DateTimeOffset day in distinctDays)
            {
                IEnumerable<Trialist> trialistsForSaidDay = trialists.Where(t => t.PreferredDay.Equals(day));
                dayTrialistPairs.Add(new DayTrialistPair(day, trialistsForSaidDay));
            }
            return dayTrialistPairs;
        }

        /// <summary>
        /// Sorts day-trialist pairs by predetermined location sorting rules
        /// </summary>
        /// <param name="dayTrialistPairs"></param>
        /// <param name="trialLocation"></param>
        /// <remarks>First day is sorted in ascending order so those with further to travel have time to settle in. Other days sorted in descending order as they can arrive day before</remarks>
        private void SortByDistance(List<DayTrialistPair> dayTrialistPairs, Gd2000Coordinate trialLocation)
        {
            for (int i = 0; i < dayTrialistPairs.Count; i++)
            {
                // For the first day, those farther away should be run later. i == 1 as no preference day is first
                // For days after this, those from farther away should be run earlier so that they can
                // Arrive the previous day, and leave earlier on the day of their runs
                if (i == 1)
                {
                    dayTrialistPairs[i].Trialists = dayTrialistPairs[i].Trialists
                    .OrderBy(t => Gd2000Coordinate.DistanceTo(t.Location, trialLocation));
                }
                else
                {
                    dayTrialistPairs[i].Trialists = dayTrialistPairs[i].Trialists
                    .OrderByDescending(t => Gd2000Coordinate.DistanceTo(t.Location, trialLocation));
                }
            }
        }

        /// <summary>
        /// Creates a final list of trialists which is ready to be run-sorted
        /// </summary>
        /// <param name="dayTrialistPairs"></param>
        /// <param name="maxRunsPerDay"></param>
        /// <returns></returns>
        /// <remarks>Fills days with trialists if said day is needing more runs to reach the maximum</remarks>
        private IEnumerable<Trialist> GenerateFinalList(List<DayTrialistPair> dayTrialistPairs, int maxRunsPerDay)
        {
            IEnumerable<Trialist> finalList = null;
            int noPrefPositionCounter = 0;

            // Take and remove the no preference pair
            DayTrialistPair noPreferencePair = dayTrialistPairs.First(p => p.Day == NO_PREFERRED_DAY_MARKER);
            dayTrialistPairs.Remove(noPreferencePair);
            List<Trialist> noPreferredDay = noPreferencePair.Trialists.ToList();

            foreach (DayTrialistPair pair in dayTrialistPairs)
            {
                IEnumerable<Trialist> list = pair.Trialists;
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

            return finalList;
        }

        /// <summary>
        /// Spreads out each trialists runs and returns the generated draw entries
        /// </summary>
        /// <param name="trialists"></param>
        /// <param name="realm"></param>
        /// <param name="maxRunsPerDay"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private IEnumerable<TrialistDrawEntry> SpreadAndGenerateRuns(IEnumerable<Trialist> trialists, Realm realm, int maxRunsPerDay, int count)
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
