using IntraMessaging;
using System;

namespace UIConcepts.Core.Model.Messages
{
    public class DialogResultMessage : Message
    {
        public DialogAction Result { get; }

        public DialogResultMessage(DialogAction result, Guid senderId)
        {
            Result = result;
            Id = senderId;
        }
    }
}
