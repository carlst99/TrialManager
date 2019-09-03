using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model
{
    public static class RealmHelpers
    {
        private static Dictionary<Type, int> _currentIds = new Dictionary<Type, int>();

        public static Realm GetRealmInstance()
        {
            string assemPath = Assembly.GetEntryAssembly().Location;
            assemPath = Path.GetDirectoryName(assemPath);
            string realmPath = Path.Combine(assemPath, "TrialManager.realm");

            RealmConfiguration config = new RealmConfiguration(realmPath)
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

            int id = ++_currentIds[typeof(T)];
            return id;
        }
    }
}
