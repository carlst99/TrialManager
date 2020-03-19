using System.Collections.Generic;
using TrialManager.Model.LocationDb;

namespace TrialManager.Services
{
    public interface ILocationService
    {
        /// <summary>
        /// Returns an autocomplete suggestion list
        /// </summary>
        /// <param name="text">The prompt text</param>
        /// <param name="maxCount">The maximum number of suggestions to return</param>
        /// <returns></returns>
        List<string> GetAutoCompleteSuggestions(string text, int maxCount = 5);
        bool TryResolve(string text, out ILocation location);
    }
}
