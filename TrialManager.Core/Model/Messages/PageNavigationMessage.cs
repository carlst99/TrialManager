using IntraMessaging;
using System;

namespace TrialManager.Core.Model.Messages
{
    public class PageNavigationMessage : Message
    {
        public Type PageType { get; set; }
    }
}
