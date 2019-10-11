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

            IEnumerable<Trialist> trialists = realm.All<Trialist>();
            trialists = trialists.OrderByDescending(t => t.Dogs.Count);

            DateTimeOffset fridayDate = new DateTimeOffset(2019, 9, 27, 7, 0, 0, TimeSpan.Zero);
            DateTimeOffset saturdayDate = new DateTimeOffset(2019, 9, 28, 7, 0, 0, TimeSpan.Zero);
            var noPref = trialists.Where(t => t.PreferredDay == DateTimeOffset.MinValue);
            var friday = trialists.Where(t => t.PreferredDay == fridayDate);
            var saturday = trialists.Where(t => t.PreferredDay == saturdayDate);

            if (location != null)
            {
                trialists = SortByDistance(trialists, location.Location);
                noPref = SortByDistance(noPref, location.Location);
                friday = SortByDistance(friday, location.Location);
                saturday = SortByDistance(saturday, location.Location);
            }

            //foreach (TrialistDrawEntry value in SortGeneric(trialists, realm, maxRunsPerDay, startDay))
            //    yield return value;
            foreach (TrialistDrawEntry value in SortGeneric(noPref, realm, maxRunsPerDay, DateTimeOffset.MinValue, 0))
                yield return value;
            int dogCount = 0;
            foreach (var element in noPref)
                dogCount += element.Dogs.Count;
            foreach (TrialistDrawEntry value in SortGeneric(friday, realm, maxRunsPerDay, fridayDate, dogCount))
                yield return value;
            foreach (var element in friday)
                dogCount += element.Dogs.Count;
            foreach (TrialistDrawEntry value in SortGeneric(saturday, realm, maxRunsPerDay, saturdayDate, dogCount))
                yield return value;
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

        private IEnumerable<TrialistDrawEntry> SortGeneric(IEnumerable<Trialist> trialists, Realm realm, int maxRunsPerDay, DateTimeOffset startDay, int count)
        {
            TrialistDrawEntry[] draw = new TrialistDrawEntry[realm.All<Dog>().Count() * 2];
            HashSet<int> usedNumbers = new HashSet<int>();
            //int count = 0;
            int startCount = count;

            foreach (Trialist element in trialists)
            {
                // Search for the next available number and update day if required
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
                    draw[localCount] = new TrialistDrawEntry(element, element.Dogs[i], localCount + 1, startDay);
                    usedNumbers.Add(localCount);

                    // Increment day if necessary and find next local count
                    localCount += DOG_RUN_SEPARATION;
                    if (localCount != 0 && maxRunsPerDay / localCount < 1)
                        dayIncrements++;
                    FindNextAvailable(usedNumbers, ref localCount);
                }
            }

            DateTimeOffset localDay = startDay;
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
                    element.Day = localDay;
                    yield return element;
                }

                // Increment the day if required
                //if (i != 0 && i % maxRunsPerDay == 0)
                //    localDay = localDay.AddDays(1);
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
