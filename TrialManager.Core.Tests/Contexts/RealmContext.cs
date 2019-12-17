using Nito.AsyncEx;
using Realms;
using System;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Tests.Contexts
{
    public static class RealmContext
    {
        public static void RunInMemory(Action<Realm> run)
        {
            AsyncContext.Run(() =>
            {
                InMemoryConfiguration config = new InMemoryConfiguration("inMemoryRealm")
                {
                    ObjectClasses = new Type[] { typeof(Trialist), typeof(Dog) }
                };
                using (Realm realm = Realm.GetInstance(config))
                    run.Invoke(realm);
                Realm.DeleteRealm(config);
            });
        }

        public static void RunOnDisk(Action<Realm> run)
        {
            AsyncContext.Run(() =>
            {
                RealmConfiguration config = new RealmConfiguration(".\\trialmanagerTestRealm.realm")
                {
                    ObjectClasses = new Type[] { typeof(Trialist), typeof(Dog) }
                };
                using (Realm realm = Realm.GetInstance(config))
                    run.Invoke(realm);
                Realm.DeleteRealm(config);
            });
        }
    }
}
