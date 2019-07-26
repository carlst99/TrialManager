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

        public enum DialogResult
        {
            Failed, Succeeded
        }

        public DialogType Type { get; set; }
        public string Title { get; set; }
        public Action<DialogResult, string> Callback { get; set; }

        public FileDialogMessage()
        {
            Type = DialogType.File;
        }
    }
}
