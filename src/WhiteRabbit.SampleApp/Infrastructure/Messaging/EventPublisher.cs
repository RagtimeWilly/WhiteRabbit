using System;
using WhiteRabbit.SampleApp.Contracts;
using WhiteRabbit.SampleApp.Interfaces;

namespace WhiteRabbit.SampleApp.Infrastructure.Messaging
{
    public class EventPublisher : IMessageAddedPublisher, IMessageUpdatedPublisher
    {
        private readonly IPublisher _publisher;
        private readonly IMessageContext _messageContext;
        private readonly string _contentType;

        public EventPublisher(IPublisher publisher, IMessageContext messageContext, ContentType contentType)
        {
            _publisher = publisher;
            _messageContext = messageContext;
            _contentType = contentType.Type;
        }

        public void PublishMessageAdded(Guid messageId, string text)
        {
            var evt = new MessageAdded
            {
                MessageId = messageId,
                Text = text
            };

            _publisher.Publish(evt, _messageContext.ReplyTo ?? string.Empty, new Guid(_messageContext.CorrelationId), _contentType);

            Console.WriteLine($"MessageAdded published. Id={messageId}, CorrelationId = {_messageContext.CorrelationId}");

        }

        public void PublishMessageUpdated(Guid messageId, string text)
        {
            var evt = new MessageUpdated
            {
                MessageId = messageId,
                UpdatedText = text
            };

            _publisher.Publish(evt, _messageContext.ReplyTo ?? string.Empty, new Guid(_messageContext.CorrelationId), _contentType);

            Console.WriteLine($"MessageUpdated published. Id={messageId}, CorrelationId = {_messageContext.CorrelationId}");
        }
    }
}
