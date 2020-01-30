using Realms;
using System;
using System.Collections.Generic;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Model
{
    public static class RealmHelpers
    {
        private static readonly Dictionary<Type, int> _currentIds = new Dictionary<Type, int>();

        public static Realm GetRealmInstance()
        {
            InMemoryConfiguration config = new InMemoryConfiguration("trialists")
            {
                ObjectClasses = new[] { typeof(Trialist), typeof(Dog) }
            };
            return Realm.GetInstance(config);
        }

        public static int GetNextId<T>(Realm realm = null) where T : RealmObject, IContextItem
        {
            if (realm == null)
                realm = GetRealmInstance();

            if (!_currentIds.ContainsKey(typeof(T)))
            {
                int max = 0;
                foreach (T element in realm.All<T>())
                {
                    if (element.Id > max)
                        max = element.Id;
                }
                _currentIds.Add(typeof(T), max);
            }

            return ++_currentIds[typeof(T)];
        }

        public static void ClearNextIds() => _currentIds.Clear();
    }
}
