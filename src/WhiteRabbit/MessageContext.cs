using System.Collections.Generic;
using RabbitMQ.Client;

namespace WhiteRabbit
{
    public struct MessageContext : IMessageContext
    {
        public MessageContext(IBasicProperties basicProperties)
        {
            MessageId = basicProperties.MessageId;
            CorrelationId = basicProperties.CorrelationId;
            ReplyTo = basicProperties.ReplyTo;
            Type = basicProperties.Type;
            ContentType = basicProperties.ContentType;
            ContentEncoding = basicProperties.ContentEncoding;
            Headers = basicProperties.Headers;
        }

        public string MessageId { get; }

        public string CorrelationId { get; }

        public string Type { get; }

        public string ReplyTo { get; }

        public string ContentType { get; }

        public string ContentEncoding { get; }

        public IDictionary<string, object> Headers { get; }
    }
}
