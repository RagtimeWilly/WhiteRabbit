using System;
using System.Collections.Generic;
using Autofac;

namespace WhiteRabbit.Autofac.Scoping
{
    public class ScopedHandlerChain : HandlerChain
    {
        private string _innerHandlerName;

        internal ScopedHandlerChain(ContainerBuilder builder)
            :base(builder)
        {
        }

        public override HandlerChain StartWith<T>()
        {
            if (ChainStarted)
                throw new InvalidOperationException("Chain has already been started with 'ScopedMessageHandler'");

            _innerHandlerName = Guid.NewGuid().ToString();

            Builder
                .RegisterType<T>()
                .Named<T>(_innerHandlerName);

            Builder
                .RegisterType<ScopedMessageHandler>()
                .WithParameter("innerHandlerName", _innerHandlerName)
                .WithParameter("innerHandlerType", typeof(T))
                .As<IMessageHandler>();

            Handlers = new List<Type> { typeof(ScopedMessageHandler), typeof(T) };

            ChainStarted = true;

            return this;
        }

        public override HandlerChain Then<THandler>()
        {
            if (!ChainStarted)
                throw new InvalidOperationException("StartWith<T> must be called before adding additional handlers to the chain");

            Then(typeof(THandler), Handlers.Count == 2 ? _innerHandlerName : "");

            return this;
        }
    }
}
