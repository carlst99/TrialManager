using IntraMessaging;
using System;

namespace TrialManager.Core.Model.Messages
{
    public class DialogMessage : Message
    {
        [Flags]
        public enum DialogButton
        {
            None = 0,
            Ok = 2,
            Yes = 4,
            No = 8,
            Cancel = 16
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public DialogButton Buttons { get; set; }
        public Action<DialogButton> Callback { get; set; }
    }
}
