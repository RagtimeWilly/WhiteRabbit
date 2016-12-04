using System;
using System.Collections.Generic;
using System.Linq;

namespace WhiteRabbit
{
    public abstract class DispatcherBase<T> : IDispatcher
    {
        protected readonly Type GenericHandler;
        protected readonly Dictionary<Type, T> Handlers;

        protected DispatcherBase(Type genericHandler, IEnumerable<T> handlers)
        {
            GenericHandler = genericHandler;

            Handlers = new Dictionary<Type, T>();

            foreach (var handler in handlers)
                Register(handler);
        }

        public abstract void Dispatch<TMsg>(TMsg cmd); 

        protected void Register(T handler)
        {
            var cmdTypes = handler
                .GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == GenericHandler)
                .Select(i => i.GetGenericArguments()[0])
                .ToList();

            if (Handlers.Keys.Any(cmdTypes.Contains))
                throw new ArgumentException("Only one handler per type is allowed");

            foreach (var cmdType in cmdTypes)
                Handlers.Add(cmdType, handler);
        }
    }
}
