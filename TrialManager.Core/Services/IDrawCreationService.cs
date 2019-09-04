using System;
using System.Collections.Generic;
using TrialManager.Core.Model;

namespace TrialManager.Core.Services
{
    public interface IDrawCreationService
    {
        IEnumerable<TrialistDrawEntry> CreateDraw(int maxRuns, DateTime startDay);
    }
}
