using System;
using WhiteRabbit.SampleApp.Contracts;
using WhiteRabbit.SampleApp.Interfaces;

namespace WhiteRabbit.SampleApp.Application
{
    public class AddMessageHandler : ICommandHandler<AddMessage>
    {
        private readonly IMessageAddedPublisher _messageAddedPublisher;

        public AddMessageHandler(IMessageAddedPublisher messageAddedPublisher)
        {
            _messageAddedPublisher = messageAddedPublisher;
        }

        public void Handle(AddMessage cmd)
        {
            Console.WriteLine($"New message recevied: {cmd.MessageId} - {cmd.Text}");

            _messageAddedPublisher.PublishMessageAdded(cmd.MessageId, cmd.Text);
        }
    }
}
