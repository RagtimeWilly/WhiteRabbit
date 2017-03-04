using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public class VerificationConsumer
    {
        private readonly IModelFactory _modelFactory;
        private readonly ISerializerFactory _serializerFactory;
        private readonly ExchangeConfig _exchange;
        private readonly Func<string> _queueNameFactory;
        private readonly bool _declareQueue;

        public VerificationConsumer(IModelFactory modelFactory, ISerializerFactory serializerFactory, ExchangeConfig exchange)
            : this(modelFactory, serializerFactory, exchange, () => Guid.NewGuid().ToString(), true)
        {
        }

        public VerificationConsumer(IModelFactory modelFactory, ISerializerFactory serializerFactory, ExchangeConfig exchange, Func<string> queueNameFactory)
            : this(modelFactory, serializerFactory, exchange, queueNameFactory, true)
        {
        }

        public VerificationConsumer(IModelFactory modelFactory, ISerializerFactory serializerFactory, ExchangeConfig exchange, Func<string> queueNameFactory, bool declareQueue)
        {
            _modelFactory = modelFactory;
            _serializerFactory = serializerFactory;
            _exchange = exchange;
            _queueNameFactory = queueNameFactory;
            _declareQueue = declareQueue;
        }

        public Task<bool> VerifyMessageReceived<T>(Func<T, bool> verify, Guid correlationId, int millisecondsTimeout) where T : class
        {
            var t = Task.Run(() =>
            {
                var resetEvent = new ManualResetEventSlim(false);

                var verified = false;

                using (var channel = _modelFactory.CreateModel())
                {
                    var queueName = _queueNameFactory();

                    var routingConfig = new RoutingConfig(queueName);
                    var queueConfig = new QueueConfig(queueName, routingConfig, _exchange, false, true, true, null);

                    if (_declareQueue)
                    {
                        channel.DeclareAndBindQueue(queueConfig);
                    }

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (obj, evtArgs) =>
                    {
                        if (correlationId == new Guid(evtArgs.BasicProperties.CorrelationId))
                        {
                            var msg = _serializerFactory.DeserializeTo<T>(evtArgs);

                            verified = verify(msg);

                            resetEvent.Set();
                        }
                    };

                    channel.BasicConsume(queueName, true, consumer);

                    resetEvent.Wait(millisecondsTimeout);
                }

                return verified;
            });

            // Allow half a second for consumer to start
            Thread.Sleep(500);

            return t;
        }
    }
}
