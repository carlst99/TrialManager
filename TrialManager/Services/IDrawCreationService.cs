using System.Collections.Generic;
using TrialManager.Model;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Services
{
    public interface IDrawCreationService
    {
        IEnumerable<TrialistDrawEntry> CreateDraw(IEnumerable<Trialist> trialists, int maxRuns, string address, int minRunSeparation, int maxDogsPerDay);
    }
}
