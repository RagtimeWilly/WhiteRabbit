using System;

namespace WhiteRabbit.SampleApp.Contracts
{
    public class AddMessage
    {
        public Guid MessageId { get; set; }

        public string Text { get; set; }
    }
}
