using System;

namespace WhiteRabbit.SampleApp.Contracts
{
    public class UpdateMessage
    {
        public Guid MessageId { get; set; }

        public string UpdatedText { get; set; }
    }
}
