using System;
using System.Collections.Generic;

namespace UIConcepts.Core.Services.CoreMessaging
{
    public interface ICoreMessagingService
    {
        ICollection<MessageSubscriber> Subscribers { get; }

        void Enqueue<T>(object sender, T message) where T : EventArgs;

        void Subscribe(Action<object, object> callback, Guid unsubKey, Type[] requestedMessageTypes = null);
        void Unsubscribe(Guid unsubKey);
    }
}
