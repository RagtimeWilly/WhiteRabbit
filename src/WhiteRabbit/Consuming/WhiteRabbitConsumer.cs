using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public class WhiteRabbitConsumer : ConsumerBase, IConsumer
    {
        private readonly IMessageHandler _handler;
        private readonly Action<Exception, string> _onError;

        public WhiteRabbitConsumer(IMessageHandler handler, IModelFactory modelFactory, string queueName, Action<Exception, string> onError)
            : base(modelFactory, queueName, onError)
        {
            _handler = handler;
            _onError = onError;
        }

        public async Task Start(bool noAck = true, bool requeueNacked = false)
        {
            Action<object, BasicDeliverEventArgs> onReceived = (obj, args) =>
            {
                try
                {
                    _handler.Handle(args);

                    if (!noAck)
                        AckMessage(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _onError(ex, $"Failed to process MessageId={args.BasicProperties?.MessageId}, CorrelationId={args.BasicProperties?.CorrelationId}, Type={args.BasicProperties?.Type}");

                    if (!noAck)
                        NackMessage(args.DeliveryTag, false, requeueNacked);
                }
            };

            await base.Start(noAck, onReceived);
        }
    }
}
