using System;
using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public class DispatchingMessageHandler : IMessageHandler
    {
        private readonly ISerializer _serializer;
        private readonly IDispatcher _dispatcher;

        public DispatchingMessageHandler(ISerializer serializer, IDispatcher dispatcher)
        {
            _serializer = serializer;
            _dispatcher = dispatcher;
        }

        public void Handle(BasicDeliverEventArgs args)
        {
            var typeProperty = args.BasicProperties.Type;

            if (typeProperty == null)
                throw new Exception("Unable to process message with unknown type");

            var type = Type.GetType(typeProperty);

            if (type == null)
                throw new Exception($"Unable to get type for {typeProperty}");

            var msg = _serializer.DeserializeObject(args.Body, type);

            _dispatcher.Dispatch(Convert.ChangeType(msg, type));
        }
    }
}
