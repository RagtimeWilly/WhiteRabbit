using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public class BasicConsumer : ConsumerBase, IConsumer
    {
        private readonly IMessageHandler _handler;

        public BasicConsumer(IMessageHandler handler, IModelFactory modelFactory, string queueName, Action<Exception, string> onError)
            : base(modelFactory, queueName, onError)
        {
            _handler = handler;
        }

        public async Task Start(bool noAck)
        {
            Action<object, BasicDeliverEventArgs> onReceived = (obj, args) =>
            {
                _handler.Handle(args);
            };

            await base.Start(noAck, onReceived);
        }
    }
}
