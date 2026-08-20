using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Messenger
{
    public static class Messenger
    {
        private static readonly Dictionary<Type, HashSet<IMessageListener>> Listeners = new();
        private static readonly Dictionary<Type, object> Messages = new();

        public static void Send<T>() where T : new()
        {
            if (!Messages.TryGetValue(typeof(T), out var message))
            {
                message = new T();
                Messages[typeof(T)] = message;
            }
            Send((T) message);
        }

        public static void Send<TMessage>(TMessage message)
        {
            if (Listeners.TryGetValue(typeof(TMessage), out var listeners))
            {
                foreach (var listener in listeners)
                {
                    ((IMessageListener<TMessage>)listener).OnMessage(message);
                }
            }
            else
            {
                Debug.LogError($"No listener is registered to {typeof(TMessage).Name}");
            }
        }

        public static void Subscribe<TMessage>(IMessageListener<TMessage> messageListener)
        {
            if (Listeners.TryGetValue(typeof(TMessage), out var receivers))
            {
                if (receivers.Contains(messageListener))
                {
                    Debug.LogError($"{messageListener.GetType().Name} already subscribed to {typeof(TMessage).Name}");
                    return;
                }
                receivers.Add(messageListener);
            }
            else
            {
                Listeners.Add(typeof(TMessage), new HashSet<IMessageListener> {messageListener});
            }
        }

        public static void Unsubscribe<TMessage>(IMessageListener<TMessage> messageListener)
        {
            if (Listeners.TryGetValue(typeof(TMessage), out var receivers))
            {
                receivers.Remove(messageListener);
            }
            else
            {
                Debug.LogError($"{messageListener.GetType().Name} is not registered for {typeof(TMessage).Name}");
            }
        }
    }
}