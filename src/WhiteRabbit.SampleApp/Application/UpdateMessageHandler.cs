using System;
using WhiteRabbit.SampleApp.Contracts;
using WhiteRabbit.SampleApp.Interfaces;

namespace WhiteRabbit.SampleApp.Application
{
    public class UpdateMessageHandler : ICommandHandler<UpdateMessage>
    {
        private readonly IMessageUpdatedPublisher _messageUpdatedPublisher;

        public UpdateMessageHandler(IMessageUpdatedPublisher messageUpdatedPublisher)
        {
            _messageUpdatedPublisher = messageUpdatedPublisher;
        }

        public void Handle(UpdateMessage cmd)
        {
            Console.WriteLine($"Message updated: {cmd.MessageId} - {cmd.UpdatedText}");

            _messageUpdatedPublisher.PublishMessageUpdated(cmd.MessageId, cmd.UpdatedText);
        }
    }
}
