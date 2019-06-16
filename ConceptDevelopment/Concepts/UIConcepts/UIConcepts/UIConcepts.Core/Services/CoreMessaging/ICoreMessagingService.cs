using System;
using System.Collections.Generic;

namespace UIConcepts.Core.Services.CoreMessaging
{
    public interface ICoreMessagingService
    {
        ICollection<MessageSubscriber> Subscribers { get; }

        void Enqueue<T>(T message) where T : ICoreMessage;
        void Subscribe(Action<ICoreMessage> callback, Guid unsubKey, Type[] requestedMessageTypes = null);
        void Unsubscribe(Guid unsubKey);
    }
}
