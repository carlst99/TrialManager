﻿using Realms;
using System;
using System.Linq;
using TrialManager.Model;

namespace TrialManager.Utils
{
    public static class RealmHelpers
    {
        public static Realm GetRealmInstance()
        {
            string realmPath = Bootstrapper.GetAppdataFilePath("TrialManager.realm");

            RealmConfiguration config = new RealmConfiguration(realmPath)
            {
                ObjectClasses = new Type[] {typeof(Preferences)}
            };
            return Realm.GetInstance(config);
        }

        public static Preferences GetUserPreferences(Realm instance = null)
        {
            if (instance is null)
                instance = GetRealmInstance();
            IQueryable<Preferences> preferences = instance.All<Preferences>();

            if (preferences.Count() == 0)
            {
                Preferences p = new Preferences();
                instance.Write(() => instance.Add(p));
                return p;
            }
            else
            {
                return preferences.First();
            }
        }
    }
}
