using Autofac;
using RabbitMQ.Client.Events;

namespace WhiteRabbit.Autofac
{
    internal class ScopedMessageHandler : IMessageHandler
    {
        private readonly string _innerHandlerName;
        private readonly ILifetimeScope _scope;

        public ScopedMessageHandler(string innerHandlerName, ILifetimeScope scope)
        {
            _innerHandlerName = innerHandlerName;
            _scope = scope;
        }

        public void Handle(BasicDeliverEventArgs args)
        {
            using (var scope = _scope.BeginLifetimeScope())
            {
                var contextProvider = scope.Resolve<MessageContextProvider>();

                contextProvider.SetContext(new MessageContext(args.BasicProperties));

                var handler = scope.ResolveNamed<IMessageHandler>(_innerHandlerName);

                handler.Handle(args);
            }
        }
    }
}
