using System;

namespace UIConcepts.Core.ViewModels.Base
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
