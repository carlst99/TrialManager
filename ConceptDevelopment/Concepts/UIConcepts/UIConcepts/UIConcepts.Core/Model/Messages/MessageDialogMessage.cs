using IntraMessaging;

namespace UIConcepts.Core.Model.Messages
{
    public class MessageDialogMessage : Message
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DialogAction Action { get; set; }
    }
}
