using System;

namespace WhiteRabbit.SampleApp.Interfaces
{
    public interface IMessageUpdatedPublisher
    {
        void PublishMessageUpdated(Guid messageId, string text);
    }
}
