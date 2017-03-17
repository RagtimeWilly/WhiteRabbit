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

        public void Handle<T>(T msg)
        {
            var args = msg as BasicDeliverEventArgs;

            if (args == null)
                throw new InvalidCastException($"Could not cast {typeof(T).Name} to BasicDeliverEventArgs");

            if (string.IsNullOrEmpty(args.BasicProperties.Type))
                throw new Exception("No type set on message");

            var type = Type.GetType(args.BasicProperties.Type);

            if (type == null)
                throw new Exception($"Unable to get type for {args.BasicProperties.Type}");

            var data = _serializerFactory.DeserializeToType(args, type);

            _dispatcher.Dispatch(Convert.ChangeType(data, type));
        }
    }
}
