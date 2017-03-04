using System;
using System.Runtime.Serialization;

namespace WhiteRabbit.Hopper.SampleApp.Definitions
{
    [DataContract]
    public class ExampleTypeA
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }

        [DataMember(Order = 3)]
        public int Number { get; set; }
    }
}
