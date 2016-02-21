using System;

namespace WhiteRabbit.SampleApp.Contracts
{
    public class MessageUpdated
    {
        public Guid MessageId { get; set; }

        public string UpdatedText { get; set; }
    }
}
