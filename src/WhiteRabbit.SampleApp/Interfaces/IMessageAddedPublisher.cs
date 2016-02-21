using System;

namespace WhiteRabbit.SampleApp.Interfaces
{
    public interface IMessageAddedPublisher
    {
        void PublishMessageAdded(Guid messageId, string text);
    }
}
