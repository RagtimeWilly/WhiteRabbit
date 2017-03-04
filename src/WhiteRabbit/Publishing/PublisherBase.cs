using System;
using System.Collections.Generic;
using System.Globalization;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace WhiteRabbit
{
    public class PublisherBase : IDisposable
    {
        private readonly IModelFactory _modelFactory;
        private readonly ISerializerFactory _serializerFactory;
        private readonly Func<string> _messageIdFactory;
        private readonly Func<IDictionary<string, object>> _headerFactory;
        private readonly TimeSpan _messageTimeToLive;

        private IModel _channel;

        protected PublisherBase(IModelFactory modelFactory, ISerializerFactory serializerFactory, string exchangeName)
            : this(modelFactory, serializerFactory, exchangeName, () => Guid.NewGuid().ToString(), () => null)
        {
        }

        protected PublisherBase(IModelFactory modelFactory, ISerializerFactory serializerFactory, string exchangeName, Func<string> messageIdFactory)
            : this(modelFactory, serializerFactory, exchangeName, messageIdFactory, () => null)
        {
        }


        protected PublisherBase(IModelFactory modelFactory, ISerializerFactory serializerFactory, string exchangeName, Func<IDictionary<string, object>> headerFactory)
            : this(modelFactory, serializerFactory, exchangeName, () => Guid.NewGuid().ToString(), headerFactory)
        {
        }

        protected PublisherBase(IModelFactory modelFactory, ISerializerFactory serializerFactory, string exchangeName, Func<string> messageIdFactory, Func<IDictionary<string, object>> headerFactory)
            : this(modelFactory, serializerFactory, exchangeName, messageIdFactory, headerFactory, TimeSpan.FromHours(1))
        {
        }

        public PublisherBase(IModelFactory modelFactory, ISerializerFactory serializerFactory, string exchangeName, Func<string> messageIdFactory, Func<IDictionary<string, object>> headerFactory,
            TimeSpan messageTimeToLive)
        {
            _modelFactory = modelFactory;
            _serializerFactory = serializerFactory;
            ExchangeName = exchangeName;
            _messageIdFactory = messageIdFactory;
            _headerFactory = headerFactory;
            _messageTimeToLive = messageTimeToLive;
        }

        protected string ExchangeName { get; }

        protected void PublishMessage<T>(T msg, string routingKey, Guid correlationId, string contentType)
        {
            PublishMessage(msg, routingKey, correlationId, contentType, props => props);
        }

        protected void PublishMessage<T>(T msg, string routingKey, Guid correlationId, string contentType, Func<BasicProperties, BasicProperties> addProps)
        {
            var payload = _serializerFactory.Serialize(msg, contentType);

            var props = new BasicProperties
            {
                ContentType = contentType,
                CorrelationId = correlationId.ToString(),
                MessageId = _messageIdFactory(),
                Headers = _headerFactory(),
                Type = msg.GetType().AssemblyQualifiedName,
                Expiration = _messageTimeToLive.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
            };

            props = addProps(props);

            if (_channel == null || _channel.IsClosed)
            {
                _channel = _modelFactory.CreateModel();
            }

            _channel.BasicPublish(ExchangeName, routingKey, props, payload);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
