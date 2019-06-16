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

        public void Enqueue<T>(object sender, T message) where T : EventArgs
        {
            foreach (MessageSubscriber subscriber in Subscribers)
                subscriber.InitiateCallback(sender, message);
        }

        public async Task EnqueueAsync<T>(object sender, T message) where T : EventArgs
        {
            await Task.Factory.StartNew(() => Enqueue(sender, message)).ConfigureAwait(false);
            return;
        }

        public void Subscribe(Action<object, object> callback, Guid unsubKey, Type[] requestedMessageTypes = null)
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
