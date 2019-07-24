using IntraMessaging;
using System;

namespace TrialManager.Core.Model.Messages
{
    public class MessageDialogMessage : Message
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DialogAction Actions { get; set; }
        public Action<DialogAction> Callback { get; set; }
    }
}
