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

            if (location != null)
            {
                foreach (TrialistDrawEntry element in SortWithLocation(realm, maxRunsPerDay, startDay, location.Location))
                    yield return element;
            } else
            {
                foreach (TrialistDrawEntry element in SortGeneric(realm, maxRunsPerDay, startDay))
                    yield return element;
            }
        }

        private IEnumerable<TrialistDrawEntry> SortWithLocation(Realm realm, int maxRunsPerDay, DateTime startDay, Location trialLocation)
        {
            int count = 1;

            // Filter all local trialists
            foreach (Trialist element in realm.All<Trialist>().Where(t => Location.DistanceTo(trialLocation, t.Location) < LOCAL_DISTANCE_MAX))
            {
                foreach (Dog dog in element.Dogs)
                    yield return new TrialistDrawEntry(element, dog, count++, startDay);
            }

            // Sort all non-local trialists
            foreach (Trialist element in realm.All<Trialist>().Where(t => Location.DistanceTo(trialLocation, t.Location) > LOCAL_DISTANCE_MAX))
            {
                foreach (Dog dog in element.Dogs)
                    yield return new TrialistDrawEntry(element, dog, count++, startDay);
            }
        }

        private IEnumerable<TrialistDrawEntry> SortGeneric(Realm realm, int maxRunsPerDay, DateTime startDay)
        {
            int count = 1;
            foreach (Trialist element in realm.All<Trialist>())
            {
                foreach (Dog dog in element.Dogs)
                    yield return new TrialistDrawEntry(element, dog, count++, startDay);
            }
        }
    }
}
