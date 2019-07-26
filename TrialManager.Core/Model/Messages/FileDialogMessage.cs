using IntraMessaging;
using System;

namespace TrialManager.Core.Model.Messages
{
    public class FileDialogMessage : Message
    {
        public enum DialogType
        {
            File, Folder
        }

        public DialogType Type { get; set; }
        public string Title { get; set; }
        public Action<string> Callback { get; set; }
    }
}
