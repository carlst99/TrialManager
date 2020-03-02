using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TrialManager.Model;
using TrialManager.Model.LocationDb;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Services
{
    public class DrawCreationService : IDrawCreationService
    {
        #region Constants

        /// <summary>
        /// Defines the maximum Gd2000 that a trialist can be considered as local for
        /// </summary>
        public const double LOCAL_DISTANCE_MAX = 0.6;

        /// <summary>
        /// Gets the marker used to define a non-preference of day
        /// </summary>
        public static readonly DateTime NO_PREFERRED_DAY_MARKER = DateTime.MinValue;

        #endregion

        private readonly ILocationService _locationService;

        public DrawCreationService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Creates a draw given certain parameters
        /// </summary>
        /// <param name="trialists">The trialists/dogs to include in the draw</param>
        /// <param name="options"></param>
        public IEnumerable<TrialistDrawEntry> CreateDraw(IEnumerable<Trialist> trialists, DrawCreationOptions options)
        {
            _locationService.TryResolve(options.TrialAddress, out ILocation trialLocation);
            options.TrialLocation = trialLocation.Location;

            // Get trialists and order by number of dogs, so those with more dogs are run earlier in the day
            trialists = trialists.OrderByDescending(t => t.Dogs.Count);

            // Split the trialist list by day
            List<DayTrialistPair> dayTrialistPairs = GetDayTrialistPairs(trialists);

            // Sort each list by distance to trial grounds
            if (trialLocation != null)
                SortByDistance(dayTrialistPairs, options);

            // Fill days if needed and generate final list
            IEnumerable<Trialist> finalList = GenerateFinalList(dayTrialistPairs, options.MaxRunsPerDay);

            foreach (TrialistDrawEntry value in SpreadAndGenerateRuns(finalList, options.MaxRunsPerDay, 0, options.MinRunSeparation, options.MaxDogsPerDay))
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
            List<DateTime> distinctDays = trialists.Distinct(new PreferredDayEqualityComparer())
                                                                .OrderBy(t => t.PreferredDay)
                                                                .Select(t => t.PreferredDay)
                                                                .ToList();
            // Setup list of trialists for each day
            foreach (DateTime day in distinctDays)
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
        private void SortByDistance(List<DayTrialistPair> dayTrialistPairs, DrawCreationOptions options)
        {
            // An improved take function, factoring in dog run count and removing from the list
            static List<Trialist> TakeAndRemove(List<Trialist> trialists, int runCount)
            {
                List<Trialist> takeList = new List<Trialist>();
                int index = 0;
                int runIndex = 0;
                while (index < trialists.Count && runIndex < runCount)
                {
                    Trialist trialist = trialists[index];
                    takeList.Add(trialist);
                    runIndex += trialist.Dogs.Count;
                    index++;
                }
                trialists.RemoveRange(0, index);
                return takeList;
            }

            if (dayTrialistPairs.Count == 0)
            {
                ArgumentException ex = new ArgumentOutOfRangeException(nameof(dayTrialistPairs), "Must have at least one day to sort");
                Log.Error(ex, "Could not create draw");
                throw ex;
            }
            else if (dayTrialistPairs.Count == 1)
            {
                List<Trialist> trialists = dayTrialistPairs[0].Trialists.ToList();
                int totalRuns = trialists.Sum(t => t.Dogs.Count);
                int totalDays = (int)Math.Ceiling((double)totalRuns / options.MaxRunsPerDay);

                List<Trialist> sortedTrialists = new List<Trialist>();

                // Start with the closest trialists on the first day if requested
                int startDay = 0;
                if (options.RunFurtherTrialistsLaterOnFirstDay)
                {
                    trialists = trialists.OrderBy(t => Gd2000Coordinate.DistanceTo(t.CoordinatePoint, options.TrialLocation)).ToList();
                    sortedTrialists.AddRange(TakeAndRemove(trialists, options.MaxRunsPerDay));
                    startDay = 1;
                }

                // Now add trialists so that those from further away are run earlier in the day
                // Do this while factoring in buffer runs so that trialists have time to arrive in the morning
                // First order by dog count so that those with more dogs are run in earlier days
                trialists = trialists.OrderByDescending(t => t.Dogs.Count).ToList();
                for (int i = startDay; i < totalDays; i++)
                {
                    List<Trialist> takeList = TakeAndRemove(trialists, options.MaxRunsPerDay);

                    List<Trialist> bufferRuns = TakeAndRemove(takeList.OrderBy(t => Gd2000Coordinate.DistanceTo(t.CoordinatePoint, options.TrialLocation)).ToList(), options.BufferRuns);
                    sortedTrialists.AddRange(bufferRuns);

                    sortedTrialists.AddRange(takeList.OrderByDescending(t => Gd2000Coordinate.DistanceTo(t.CoordinatePoint, options.TrialLocation)));
                }

                dayTrialistPairs[0].Trialists = sortedTrialists;
            }
            else
            {
                bool noPrefDayExists = dayTrialistPairs.Select(d => d.Day).Contains(NO_PREFERRED_DAY_MARKER);
                for (int i = 0; i < dayTrialistPairs.Count; i++)
                {
                    // For the first day, those farther away should be run later. i == 1 as no preference day is first
                    // For days after this, those from farther away should be run earlier so that they can
                    // Arrive the previous day, and leave earlier on the day of their runs
                    if (((noPrefDayExists && i == 1) || (!noPrefDayExists && i == 0)) && options.RunFurtherTrialistsLaterOnFirstDay)
                    {
                        dayTrialistPairs[i].Trialists = dayTrialistPairs[i].Trialists
                            .OrderBy(t => Gd2000Coordinate.DistanceTo(t.CoordinatePoint, options.TrialLocation));
                    }
                    else
                    {
                        // Add some buffer runs first
                        List<Trialist> sortedTrialists = new List<Trialist>();
                        List<Trialist> trialists = dayTrialistPairs[i].Trialists
                            .OrderBy(t => Gd2000Coordinate.DistanceTo(t.CoordinatePoint, options.TrialLocation))
                            .ToList();

                        List<Trialist> bufferTrialists = TakeAndRemove(trialists, options.BufferRuns);
                        sortedTrialists.AddRange(bufferTrialists);
                        sortedTrialists.AddRange(trialists.OrderByDescending(t => Gd2000Coordinate.DistanceTo(t.CoordinatePoint, options.TrialLocation)));
                        dayTrialistPairs[i].Trialists = sortedTrialists;
                    }
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
            if (dayTrialistPairs.Count == 0)
            {
                ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException(nameof(dayTrialistPairs), "At least one day is required to create a draw");
                Log.Error(ex, "Could not create draw");
                throw ex;
            }
            else if (dayTrialistPairs.Count == 1)
            {
                return dayTrialistPairs[0].Trialists;
            }
            else if (dayTrialistPairs.Select(t => t.Day).Contains(NO_PREFERRED_DAY_MARKER))
            {
                List<Trialist> finalList = new List<Trialist>();
                int noPrefPositionCounter = 0;

                // Take and remove the no preference pair
                DayTrialistPair noPreferencePair = dayTrialistPairs.First(p => p.Day.Equals(NO_PREFERRED_DAY_MARKER));
                dayTrialistPairs.Remove(noPreferencePair);
                List<Trialist> noPreferredDay = noPreferencePair.Trialists.ToList();

                foreach (DayTrialistPair pair in dayTrialistPairs)
                {
                    IEnumerable<Trialist> list = pair.Trialists;

                    // Fill up the day with no-preference trialists, if we have not yet met the maximum run count
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
                        finalList = list.ToList();
                    else
                        finalList.AddRange(list);
                }

                if (noPrefPositionCounter < noPreferredDay.Count)
                    finalList.AddRange(noPreferredDay.GetRange(noPrefPositionCounter, noPreferredDay.Count - noPrefPositionCounter));

                return finalList;
            }
            else
            {
                List<Trialist> finalList = new List<Trialist>();
                foreach (DayTrialistPair pair in dayTrialistPairs)
                    finalList.AddRange(pair.Trialists);
                return finalList;
            }
        }

        /// <summary>
        /// Spreads out each trialists runs and returns the generated draw entries
        /// </summary>
        /// <param name="trialists"></param>
        /// <param name="maxRunsPerDay"></param>
        /// <param name="count"></param>
        /// <param name="maxDogsPerDay"></param>
        /// <param name="minRunSeparation"></param>
        /// <returns></returns>
        private IEnumerable<TrialistDrawEntry> SpreadAndGenerateRuns(IEnumerable<Trialist> trialists, int maxRunsPerDay, int count, int minRunSeparation, int maxDogsPerDay)
        {
            if (maxRunsPerDay <= 0)
                throw new ArgumentException("Max Runs must be greater than 0", nameof(maxRunsPerDay));
            if (minRunSeparation < 0)
                throw new ArgumentException("Min Run Separation must be greater than -1", nameof(minRunSeparation));
            if (maxDogsPerDay <= 0)
                throw new ArgumentException("Max dogs must be greater than 0", nameof(maxDogsPerDay));

            minRunSeparation++;
            int dogCount = 0;
            foreach (Trialist trialist in trialists)
                dogCount += trialist.Dogs.Count;

            TrialistDrawEntry[] draw = new TrialistDrawEntry[dogCount * 2];
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
                    if (i != 0 && i % maxDogsPerDay == 0 && i / maxDogsPerDay == dayIncrements)
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
                    localCount += minRunSeparation;
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
