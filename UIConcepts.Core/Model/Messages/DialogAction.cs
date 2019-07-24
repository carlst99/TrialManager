using System;

namespace UIConcepts.Core.Model.Messages
{
    [Flags]
    public enum DialogAction
    {
        None = 0,
        Ok = 1,
        Cancel = 2,
        Yes = 4,
        No = 8
    }
}
