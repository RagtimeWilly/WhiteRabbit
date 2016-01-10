using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public sealed class Publisher<T> : IPublisher<T>
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ISerializer _serializer;
        private readonly ExchangeConfig _exchange;

        public Publisher(IConnectionFactory connectionFactory, ISerializer serializer, ExchangeConfig exchange)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _serializer = serializer;
            _exchange = exchange;

            _channel.DeclareExchange(_exchange);
        }

        public async Task Publish(T msg, string routingKey, Guid correlationId)
        {
            await Task.Run(() =>
            {
                var body = _serializer.Serialize(msg);

                var properties = new BasicProperties()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    CorrelationId = correlationId.ToString(),
                };

                _channel.BasicPublish(_exchange.Name, routingKey, properties, body);
            });
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
