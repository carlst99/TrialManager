using System.Collections.Generic;
using TrialManager.Model.Draw;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Services
{
    public interface IDrawCreationService
    {
        IEnumerable<TrialistDrawEntry> CreateDraw(IEnumerable<Trialist> trialists, DrawCreationOptions options);
    }
}
