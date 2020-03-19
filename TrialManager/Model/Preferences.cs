using MessagePack;
using Realms;
using TrialManager.Model.Draw;

namespace TrialManager.Model
{
    public class Preferences : RealmObject
    {
        [Required, MapTo("DrawCreationOptions")]
        private byte[] DrawCreationOptionsRaw { get; set; }

        [Ignored]
        public DrawCreationOptions DrawCreationOptions
        {
            get => MessagePackSerializer.Deserialize<DrawCreationOptions>(DrawCreationOptionsRaw);
            set => DrawCreationOptionsRaw = MessagePackSerializer.Serialize(value);
        }

        public bool IsFirstRunComplete { get; set; }
        public bool IsDiagnosticsEnabled { get; set; }

        public Preferences()
        {
            IsFirstRunComplete = false;
            IsDiagnosticsEnabled = true;
            DrawCreationOptions = new DrawCreationOptions();
        }
    }
}
