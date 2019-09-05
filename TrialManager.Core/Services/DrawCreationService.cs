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
        /// <summary>
        /// Defines the maximum Gd2000 that a trialist can be considered as local for
        /// </summary>
        public const double LOCAL_DISTANCE_MAX = 0.6;

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

            foreach (TrialistDrawEntry value in SortGeneric(trialists, maxRunsPerDay, startDay))
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

        private IEnumerable<TrialistDrawEntry> SortGeneric(IEnumerable<Trialist> trialists, int maxRunsPerDay, DateTime startDay)
        {
            int count = 1;
            foreach (Trialist element in trialists)
            {
                foreach (Dog dog in element.Dogs)
                    yield return new TrialistDrawEntry(element, dog, count++, startDay);
            }
        }
    }
}
