using System;

namespace TrialManager.Core.ViewModels.Base
{
    public class DisplayNavigationAttribute : Attribute
    {
        public readonly string DisplayName;

        public DisplayNavigationAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
