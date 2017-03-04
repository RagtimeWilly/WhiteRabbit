using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public class DelayedVerificationConsumer
    {
        private readonly IModelFactory _modelFactory;
        private readonly ISerializerFactory _serializerFactory;
        private readonly ExchangeConfig _exchangeConfig;
        private readonly Func<string> _queueNameFactory;
        private readonly bool _declareQueue;

        private readonly Dictionary<Guid, BasicDeliverEventArgs> _msgsReceived;
        private readonly ManualResetEventSlim _stopConsumer;

        public DelayedVerificationConsumer(IModelFactory modelFactory, ISerializerFactory serializerFactory, ExchangeConfig exchangeConfig, Func<string> queueNameFactory)
            : this(modelFactory, serializerFactory, exchangeConfig, queueNameFactory, true)
        {
        }

        public DelayedVerificationConsumer(IModelFactory modelFactory, ISerializerFactory serializerFactory, ExchangeConfig exchangeConfig)
            : this(modelFactory, serializerFactory, exchangeConfig, () => Guid.NewGuid().ToString(), true)
        {
        }

        public DelayedVerificationConsumer(IModelFactory modelFactory, ISerializerFactory serializerFactory, ExchangeConfig exchangeConfig, Func<string> queueNameFactory, bool declareQueue)
        {
            _modelFactory = modelFactory;
            _serializerFactory = serializerFactory;
            _exchangeConfig = exchangeConfig;
            _queueNameFactory = queueNameFactory;
            _declareQueue = declareQueue;

            _msgsReceived = new Dictionary<Guid, BasicDeliverEventArgs>();
            _stopConsumer = new ManualResetEventSlim(false);

            StartConsumer();
        }

        public T GetReceivedMessage<T>(Guid correlationId, int millisecondsTimeout) where T : class
        {
            var timeoutTask = Task.Run(() => Task.Delay(millisecondsTimeout));

            while (!timeoutTask.IsCompleted)
            {
                if (_msgsReceived.ContainsKey(correlationId))
                {
                    _stopConsumer.Set();

                    return _serializerFactory.DeserializeTo<T>(_msgsReceived[correlationId]);
                }
            }

            return null;
        }

        private void StartConsumer()
        {
            Task.Run(() =>
            {
                using (var channel = _modelFactory.CreateModel())
                {
                    var queueName = _queueNameFactory();

                    var routingConfig = new RoutingConfig(queueName);
                    var queueConfig = new QueueConfig(queueName, routingConfig, _exchangeConfig, false, true, true, null);

                    if (_declareQueue)
                    {
                        channel.DeclareAndBindQueue(queueConfig);
                    }

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (obj, evtArgs) =>
                    {
                        var correlationId = new Guid(evtArgs.BasicProperties.CorrelationId);

                        _msgsReceived.Add(correlationId, evtArgs);
                    };

                    channel.BasicConsume(queueName, true, consumer);

                    _stopConsumer.Wait();
                }
            });

            // Allow half a second for consumer to start
            Thread.Sleep(500);
        }
    }
}
