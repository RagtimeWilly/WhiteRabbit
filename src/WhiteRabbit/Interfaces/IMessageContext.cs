using System.Collections.Generic;

namespace WhiteRabbit
{
    public interface IMessageContext
    {
        string MessageId { get; }

        string CorrelationId { get; }

        string Type { get; }

        string ReplyTo { get; }

        string ContentType { get; }

        string ContentEncoding { get; }

        IDictionary<string, object> Headers { get; }
    }
}
