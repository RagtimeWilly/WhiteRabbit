using System;
using Autofac;
using RabbitMQ.Client.Events;

namespace WhiteRabbit.Autofac.Scoping
{
    internal class ScopedMessageHandler : IMessageHandler
    {
        private readonly string _innerHandlerName;
        private readonly Type _innerHandlerType;
        private readonly ILifetimeScope _scope;

        public ScopedMessageHandler(string innerHandlerName, Type innerHandlerType, ILifetimeScope scope)
        {
            _innerHandlerName = innerHandlerName;
            _innerHandlerType = innerHandlerType;
            _scope = scope;
        }

        public void Handle(BasicDeliverEventArgs args)
        {
            using (var scope = _scope.BeginLifetimeScope())
            {
                var contextProvider = scope.Resolve<MessageContextProvider>();

                contextProvider.SetContext(new MessageContext(args.BasicProperties));

                var innerHandler = scope.ResolveNamed(_innerHandlerName, _innerHandlerType) as IMessageHandler;

                if (innerHandler == null)
                    throw new InvalidCastException($"Failed to cast {_innerHandlerType} to {nameof(IMessageHandler)}. Please check it implements the correct interface.");

                innerHandler.Handle(args);
            }
        }
    }
}
