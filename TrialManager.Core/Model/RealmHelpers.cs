using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model
{
    public static class RealmHelpers
    {
        private static Dictionary<Type, int> _currentIds = new Dictionary<Type, int>();

        public static Realm GetRealmInstance()
        {
            return Realm.GetInstance(new RealmConfiguration("TrialManager.realm"));
        }

        public static int GetNextId<T>(Realm realm = null) where T : RealmObject, IContextItem
        {
            if (realm == null)
                realm = GetRealmInstance();

            if (!_currentIds.ContainsKey(typeof(T)))
                _currentIds.Add(typeof(T), realm.All<T>().Max(t => t.Id));

            realm.Dispose();
            return ++_currentIds[typeof(T)];
        }
    }
}
