using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace SwiftTailer.Wpf.Infrastructure.Messaging
{
    public class MessageBroker
    {
        private static readonly ConcurrentDictionary<Type, Action<DomainEvent>> Handlers = new ConcurrentDictionary<Type, Action<DomainEvent>>();

        public static void Subscribe(Type args, Action<DomainEvent> handler)
        {
            if (!Handlers.Any(h => h.Key == args && h.Value.Equals(handler)))
                Handlers.TryAdd(args, handler);
        }

        public static void Unsubscribe(Type args, Action<DomainEvent> handler)
        {
            Action<DomainEvent> removed;
            Handlers.TryRemove(args, out removed);
        }

        public static void Broadcast(DomainEvent args)
        {
            var handlers = Handlers.Where(h => h.Key == args.GetType());
            Debug.WriteLine($"{args.EventName} => {args.Message}");
            foreach (var handler in handlers)
            {
                handler.Value(args);
            }
        }
    }
}
