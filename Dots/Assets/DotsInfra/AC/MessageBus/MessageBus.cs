using System;
using System.Collections.Generic;
using System.Linq;
using Dots.Extras;

namespace Dots.Infra.AC
{
    public interface IMessageBus
    {
        void Post<TMessage>(TMessage message) where TMessage : IMessage;

        void Subscribe<TMessage>(Action<TMessage> action) where TMessage : IMessage;
        void Unsubscribe<TMessage>(Action<TMessage> action) where TMessage : IMessage;
    }

    public class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<object>> _messageActionsMap = new Dictionary<Type, List<object>>();

        public void Post<TMessage>(TMessage message) where TMessage : IMessage
        {
            if (_messageActionsMap.TryGetValue(typeof(TMessage), out var handlers))
            {
                handlers.OfType<Action<TMessage>>().ForEach(action => action(message));
            }
        }

        public void Subscribe<TMessage>(Action<TMessage> action) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!_messageActionsMap.TryGetValue(messageType, out var actions))
            {
                actions = new List<object>();
                _messageActionsMap.Add(messageType, actions);
            }
            
            actions.Add(action);
        }

        public void Unsubscribe<TMessage>(Action<TMessage> action) where TMessage : IMessage
        {
            if (_messageActionsMap.TryGetValue(typeof(TMessage), out var actions))
            {
                actions.Remove(action);
            }
        }
    }
}
