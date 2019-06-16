using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UIConcepts.Core.Services.CoreMessaging
{
    public sealed class CoreMessagingService : ICoreMessagingService
    {
        public static CoreMessagingService Instance { get; } = new CoreMessagingService();

        public ICollection<MessageSubscriber> Subscribers { get; }

        static CoreMessagingService() { }

        private CoreMessagingService()
        {
            Subscribers = new List<MessageSubscriber>();
        }

        public void Enqueue<T>(T message) where T : ICoreMessage
        {
            foreach (MessageSubscriber subscriber in Subscribers)
                subscriber.InitiateCallback(message);
        }

        public async Task EnqueueAsync<T>(T message) where T : ICoreMessage
        {
            await Task.Factory.StartNew(() => Enqueue(message)).ConfigureAwait(false);
            return;
        }

        public void Subscribe(Action<ICoreMessage> callback, Guid unsubKey, Type[] requestedMessageTypes = null)
        {
            Subscribers.Add(new MessageSubscriber(callback, unsubKey, requestedMessageTypes));
        }

        public void Unsubscribe(Guid unsubKey)
        {
            if (unsubKey == default || unsubKey == Guid.Empty)
                return;

            MessageSubscriber subscriber = Subscribers.FirstOrDefault(s => s.UnsubscribeKey == unsubKey);
            if (!subscriber.Equals(default))
                Subscribers.Remove(subscriber);
        }
    }
}
