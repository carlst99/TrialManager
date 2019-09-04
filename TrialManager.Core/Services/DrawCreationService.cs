using Realms;
using System;
using System.Collections.Generic;
using TrialManager.Core.Model;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Services
{
    public class DrawCreationService : IDrawCreationService
    {
        public IEnumerable<TrialistDrawEntry> CreateDraw(int maxRunsPerDay, DateTime startDay)
        {
            int count = 1;

            Realm realm = RealmHelpers.GetRealmInstance();
            foreach (Trialist element in realm.All<Trialist>())
            {
                foreach (Dog dog in element.Dogs)
                    yield return new TrialistDrawEntry(element, dog, count++, startDay);
            }
        }
    }
}
