using System;
using System.Runtime.Serialization;

namespace WhiteRabbit.Hopper.SampleApp.Definitions
{
    [DataContract]
    public class ExampleTypeB
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public int Count { get; set; }
    }
}
