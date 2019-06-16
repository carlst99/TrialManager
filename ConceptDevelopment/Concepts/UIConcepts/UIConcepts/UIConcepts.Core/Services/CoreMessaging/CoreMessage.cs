using System;

namespace UIConcepts.Core.Services.CoreMessaging
{
    public abstract class CoreMessage : ICoreMessage
    {
        public object Sender { get; }

        public Type SenderType => Sender.GetType();
    }
}
