using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace WhiteRabbit.Autofac
{
    public class HandlerChain
    {
        protected readonly ContainerBuilder Builder;

        protected List<Type> Handlers;
        protected bool ChainStarted;

        internal HandlerChain(ContainerBuilder builder)
        {
            Builder = builder;
            Handlers = new List<Type>();
            ChainStarted = false;
        }

        public virtual HandlerChain StartWith<THandler>()
        {
            return StartWith(typeof (THandler));
        }

        public virtual HandlerChain Then<THandler>()
        {
            if (!ChainStarted)
                throw new InvalidOperationException("StartWith<T> must be called before adding additional handlers to the chain");

            return Then(typeof (THandler));
        }

        internal HandlerChain StartWith(Type startingHandler)
        {
            if (ChainStarted)
                throw new InvalidOperationException($"Chain has already been started with '{Handlers.First().Name}'");

            ValidateHandlerType(startingHandler);

            Handlers = new List<Type> { startingHandler };

            ChainStarted = true;

            return this;
        }
    
        internal HandlerChain Then(Type nextHandlerType, string registrationName = "")
        {
            ValidateHandlerType(nextHandlerType);

            var method = typeof(ContainerBuilderEx).GetMethod("RegisterHandlerWithInner", BindingFlags.Static | BindingFlags.NonPublic);
            var generic = method.MakeGenericMethod(Handlers.Last(), nextHandlerType);

            generic.Invoke(this, new object[] { Builder, Handlers.Count == 1, registrationName});

            Handlers.Add(nextHandlerType);

            return this;
        }

        private void ValidateHandlerType(Type t)
        {
            if (!t.GetInterfaces().Contains(typeof(IMessageHandler)))
                throw new ArgumentException($"Handler {t.Name} cannot be part of chain as it does not implement '{nameof(IMessageHandler)}'");
        }
    }
}
