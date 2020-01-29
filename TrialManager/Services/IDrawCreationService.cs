using System;
using System.Collections.Generic;
using TrialManager.Model;

namespace TrialManager.Services
{
    public interface IDrawCreationService
    {
        IEnumerable<TrialistDrawEntry> CreateDraw(int maxRuns, DateTime startDay, string address);
    }
}
