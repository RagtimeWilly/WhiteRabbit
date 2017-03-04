using System;
using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public class DispatchingMessageHandler : IMessageHandler
    {
        private readonly ISerializerFactory _serializerFactory;
        private readonly IDispatcher _dispatcher;

        public DispatchingMessageHandler(ISerializerFactory serializerFactory, IDispatcher dispatcher)
        {
            _serializerFactory = serializerFactory;
            _dispatcher = dispatcher;
        }

        public void Handle(BasicDeliverEventArgs args)
        {
            if (string.IsNullOrEmpty(args.BasicProperties.Type))
                throw new Exception("No type set on message");

            var type = Type.GetType(args.BasicProperties.Type);

            if (type == null)
                throw new Exception($"Unable to get type for {args.BasicProperties.Type}");

            var msg = _serializerFactory.DeserializeToType(args, type);

            _dispatcher.Dispatch(Convert.ChangeType(msg, type));
        }
    }
}
