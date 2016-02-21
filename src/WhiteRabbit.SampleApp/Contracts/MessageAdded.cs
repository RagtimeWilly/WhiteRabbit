using System;

namespace WhiteRabbit.SampleApp.Contracts
{
    public class MessageAdded
    {
        public Guid MessageId { get; set; }

        public string Text { get; set; }
    }
}
