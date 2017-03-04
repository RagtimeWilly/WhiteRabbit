using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhiteRabbit.Hopper.SampleApp.Definitions;

namespace WhiteRabbit.Hopper.SampleApp.Services
{
    public class SampleService
    {
        private readonly Hopper _hopper;

        public SampleService(Hopper hopper)
        {
            _hopper = hopper;
        }

        public void Start()
        {
            var types = new List<Type>
            {
                typeof (ExampleTypeA),
                typeof (ExampleTypeB),
            };

            var contentTypes = new List<string>
            {
                "application/json",
                "application/protobuf"
            };

            Task.Run(() => _hopper.Start(types, contentTypes));
        }

        public void Stop()
        {
            _hopper.Dispose();
        }
    }
}
