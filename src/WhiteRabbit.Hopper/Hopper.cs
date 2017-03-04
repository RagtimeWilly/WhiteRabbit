using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

namespace WhiteRabbit.Hopper
{
    public class Hopper : IDisposable
    {
        private readonly IModelFactory _modelFactory;
        private readonly ISerializerFactory _serializerFactory;
        private readonly ExchangeConfig _exchange;
        private readonly Action<string> _logAction;
        private readonly IDictionary<Type, Func<object>> _typeFactory;
        private readonly TimeSpan _delayBetweenRuns;
        private readonly TimeSpan _messageTimeout;

        private IModel _publisherChannel;
        private IModel _consumerChannel;
        private bool _isRunning;

        public Hopper(IModelFactory modelFactory, ISerializerFactory serializerFactory, ExchangeConfig exchange, Action<string> logAction, IDictionary<Type, Func<object>> typeFactory, 
            TimeSpan delayBetweenRuns, TimeSpan messageTimeout)
        {
            _modelFactory = modelFactory;
            _serializerFactory = serializerFactory;
            _exchange = exchange;
            _logAction = logAction;
            _typeFactory = typeFactory;
            _delayBetweenRuns = delayBetweenRuns;
            _messageTimeout = messageTimeout;
        }

        public void Start(ICollection<Type> msgTypes, ICollection<string> contentTypes)
        {
            ValidateTypeFactoriesProvided(msgTypes);
            ValidateSerializersProvided(contentTypes);

            var msgsToTrack = contentTypes
                .SelectMany(a => msgTypes.Select(b => new Tuple<string, Type>(a, b)))
                .ToList();

            _isRunning = true;

            while (_isRunning)
            {
                foreach (var t in msgsToTrack)
                {
                    if (_publisherChannel == null || _publisherChannel.IsClosed)
                    {
                        _publisherChannel = _modelFactory.CreateModel();
                    }

                    if (_consumerChannel == null || _consumerChannel.IsClosed)
                    {
                        _consumerChannel = _modelFactory.CreateModel();
                    }

                    var serializer = _serializerFactory.For(t.Item1);
                    var msg = _typeFactory[t.Item2]();
                    var resetEvent = new ManualResetEventSlim();
                    var wasReceived = false;

                    var correlationId = Guid.NewGuid();
                    var routingKey = Guid.NewGuid().ToString();

                    var consumer = new EventingBasicConsumer(_consumerChannel);
                    var roundTripTimer = new Stopwatch();
                    var deserializationTimer = new Stopwatch();

                    consumer.Received += (obj, evtArgs) =>
                    {
                        if (correlationId == new Guid(evtArgs.BasicProperties.CorrelationId))
                        {
                            deserializationTimer.Start();

                            serializer.DeserializeObject(evtArgs.Body, t.Item2);

                            deserializationTimer.Stop();
                            roundTripTimer.Stop();

                            wasReceived = true;
                            resetEvent.Set();
                        }
                    };

                    var queueConfig = new QueueConfig($"WhiteRabbit.Hopper.{routingKey}", new RoutingConfig($"WhiteRabbit.Hopper.{routingKey}"), _exchange, false, true, true, null);
                    
                    _consumerChannel.DeclareAndBindQueue(queueConfig);
                    _consumerChannel.BasicConsume($"WhiteRabbit.Hopper.{routingKey}", true, consumer);

                    var props = new BasicProperties
                    {
                        ContentType = t.Item1,
                        CorrelationId = correlationId.ToString(),
                        MessageId = Guid.NewGuid().ToString(),
                        Type = t.Item2.AssemblyQualifiedName,
                        Expiration = _messageTimeout.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
                    };

                    roundTripTimer.Start();
                    var serializationTimer = Stopwatch.StartNew();

                    var payload = serializer.Serialize(msg);

                    serializationTimer.Stop();

                    _publisherChannel.BasicPublish(_exchange.Name, $"WhiteRabbit.Hopper.{routingKey}", props, payload);

                    resetEvent.Wait(_messageTimeout);

                    if (wasReceived)
                    {
                        _logAction($"[MessageType={t.Item2.Name}] [ContentType={t.Item1}] [MessageText=Serialization completed in {serializationTimer.Elapsed}]");
                        _logAction($"[MessageType={t.Item2.Name}] [ContentType={t.Item1}] [MessageText=De-serialization completed in {deserializationTimer.Elapsed}]");
                        _logAction($"[MessageType={t.Item2.Name}] [ContentType={t.Item1}] [MessageText=Round trip completed in {roundTripTimer.Elapsed}]");
                    }
                    else
                    {
                        _logAction($"{t.Item2.Name} was not received within the timeout {_messageTimeout}");
                    }
                }

                Task.Delay(_delayBetweenRuns).Wait();
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            _publisherChannel?.Dispose();
            _consumerChannel?.Dispose();
        }

        private void ValidateTypeFactoriesProvided(IEnumerable<Type> msgTypes)
        {
            var missingFactoryTypes = msgTypes
               .Where(t => !_typeFactory.ContainsKey(t))
               .ToList();

            if (missingFactoryTypes.Any())
                throw new Exception($"Type factories missing for: {string.Join(",", missingFactoryTypes.Select(t => t.Name))}");
        }

        private void ValidateSerializersProvided(IEnumerable<string> contentTypes)
        {
            var missingSerializers = contentTypes
               .Where(s => _serializerFactory.For(s) == null)
               .ToList();

            if (missingSerializers.Any())
                throw new Exception($"Serializers missing for content types: {string.Join(",", missingSerializers)}");
        }
    }
}
