using Realms;
using System.Collections.Generic;
using TrialManager.Core.Model.LocationDb;

namespace TrialManager.Core.Services
{
    public interface ILocationService
    {
        /// <summary>
        /// Returns an autocomplete suggestion list
        /// </summary>
        /// <param name="text">The prompt text</param>
        /// <param name="maxCount">The maximum number of suggestions to return</param>
        /// <returns></returns>
        List<string> GetAutoCompleteSuggestions(string text, int maxCount = 5, Realm realm = null);
        bool TryResolve(string text, out LocationBase location, Realm realm = null);
    }
}
