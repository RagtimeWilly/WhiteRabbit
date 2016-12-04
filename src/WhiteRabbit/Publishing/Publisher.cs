using RabbitMQ.Client.Framing;
using System;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public sealed class Publisher : IPublisher
    {
        private readonly IModelFactory _modelFactory;
        private readonly ISerializer _serializer;
        private readonly string _exchange;

        public Publisher(IModelFactory modelFactory, ISerializer serializer, string exchange)
        {
            _modelFactory = modelFactory;
            _serializer = serializer;
            _exchange = exchange;
        }

        public async Task Publish<T>(T msg, string routingKey, Guid correlationId)
        {
            await Task.Run(() =>
            {
                using (var channel = _modelFactory.CreateModel())
                {
                    var body = _serializer.Serialize(msg);

                    var properties = new BasicProperties
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        CorrelationId = correlationId.ToString(),
                        Type = msg.GetType().AssemblyQualifiedName
                    };

                    channel.BasicPublish(_exchange, routingKey, false, properties, body);
                }
            });
        }
    }
}
