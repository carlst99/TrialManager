using System;

namespace UIConcepts.Core.Services.CoreMessaging
{
    public interface ICoreMessage
    {
        object Sender { get; }
        Type SenderType { get; }
    }
}
