using System;
using System.Collections.Generic;
using System.Linq;

namespace WhiteRabbit
{
    public abstract class DispatcherBase<THandler> : IDispatcher
    {
        protected readonly Type GenericHandler;
        protected readonly Dictionary<Type, THandler> Handlers;

        protected DispatcherBase(Type genericHandler, IEnumerable<THandler> handlers)
        {
            GenericHandler = genericHandler;

            Handlers = new Dictionary<Type, THandler>();

            foreach (var handler in handlers)
                Register(handler);
        }

        public abstract void Dispatch<TMsg>(TMsg cmd); 

        protected void Register(THandler handler)
        {
            var cmdTypes = handler
                .GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == GenericHandler)
                .Select(i => i.GetGenericArguments()[0])
                .ToList();

            if (!cmdTypes.Any())
                throw new ArgumentException($"No types associated with handler: {handler.GetType()}");

            foreach (var cmdType in cmdTypes)
            {
                if (Handlers.ContainsKey(cmdType))
                    throw new ArgumentException($"Only one handler per type is allowed. Attempted to register multiple handlers of type: {cmdType}");

                Handlers.Add(cmdType, handler);
            }
        }
    }
}
