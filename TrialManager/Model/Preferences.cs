using MessagePack;
using Realms;
using TrialManager.Model.Draw;

namespace TrialManager.Model
{
    public class Preferences : RealmObject
    {
        [Required]
        private byte[] DrawCreationOptionsRaw { get; set; }

        [Ignored]
        public DrawCreationOptions DrawCreationOptions
        {
            get => MessagePackSerializer.Deserialize<DrawCreationOptions>(DrawCreationOptionsRaw);
            set => DrawCreationOptionsRaw = MessagePackSerializer.Serialize(value);
        }

        [Required]
        public bool IsFirstRunComplete { get; set; }

        [Required]
        public bool IsDiagnosticsEnabled { get; set; }
    }
}
